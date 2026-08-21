#!/usr/bin/env python3
"""Extracts id -> {name, category, type} from a decompressed Data/Objects .xnb body.

Data/Objects is a Dictionary<string, ObjectData>, and ObjectData is written by XNA's
ReflectiveReader, so there is no self-describing layout to follow. What is stable is
the start of every entry: the key, then the object's first four string fields (Name,
DisplayName, Description, Type) and then Category as an int32. That is all this needs,
so it scans for that shape rather than modelling the whole type.

Feed it the same decompressed body xnb_dict.py takes (see that file for how to get it).

Usage: python3 tools/xnb_objects.py Objects.bin Objects.json
"""
import json, re, struct, sys

STRING_READER = 0x02   # index of StringReader in Data/Objects' type reader manifest
OBJECT_READER = 0x03   # index of ReflectiveReader<ObjectData>


def read(path):
    data = open(path, "rb").read()

    def seven_bit_int(pos):
        result = shift = 0
        while True:
            byte = data[pos]
            pos += 1
            result |= (byte & 0x7F) << shift
            shift += 7
            if not byte & 0x80:
                return result, pos

    def string(pos):
        """A string field: its type-reader index, then a length-prefixed body."""
        if data[pos] != STRING_READER:
            return None, pos
        length, pos = seven_bit_int(pos + 1)
        try:
            return data[pos:pos + length].decode("utf-8"), pos + length
        except UnicodeDecodeError:
            return None, pos

    result = {}
    for match in re.finditer(bytes([STRING_READER]), data):
        item_id, pos = string(match.start())
        if item_id is None or not 1 <= len(item_id) <= 64:
            continue
        if data[pos:pos + 1] != bytes([OBJECT_READER]):
            continue
        pos += 1
        fields = []
        for _ in range(4):  # Name, DisplayName, Description, Type
            value, pos = string(pos)
            if value is None:
                break
            fields.append(value)
        if len(fields) < 4 or pos + 4 > len(data):
            continue
        category = struct.unpack_from("<i", data, pos)[0]
        if -200 < category < 200:  # a plausible category, so the layout still lines up
            result.setdefault(item_id, {"name": fields[0], "category": category,
                                        "type": fields[3]})
    return result


if __name__ == "__main__":
    json.dump(read(sys.argv[1]), open(sys.argv[2], "w", encoding="utf-8"),
              indent=1, ensure_ascii=False)
