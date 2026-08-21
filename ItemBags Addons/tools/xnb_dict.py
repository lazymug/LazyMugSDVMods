#!/usr/bin/env python3
"""Reads a Dictionary<string, string> out of an already-decompressed .xnb body.

The game ships Content as LZX-compressed .xnb, which Python can't unpack on its
own - run the file through xnbcli (or any XNB tool) first and feed the raw body
here. Used to get Data/CraftingRecipes for gen_bagconfig.py.

Usage: python3 tools/xnb_dict.py CraftingRecipes.bin CraftingRecipes.json
"""
import json, struct, sys


class Reader:
    def __init__(self, data):
        self.data, self.pos = data, 0

    def byte(self):
        value = self.data[self.pos]
        self.pos += 1
        return value

    def int32(self):
        value = struct.unpack_from("<i", self.data, self.pos)[0]
        self.pos += 4
        return value

    def uint32(self):
        value = struct.unpack_from("<I", self.data, self.pos)[0]
        self.pos += 4
        return value

    def seven_bit_int(self):
        result = shift = 0
        while True:
            byte = self.byte()
            result |= (byte & 0x7F) << shift
            shift += 7
            if not byte & 0x80:
                return result

    def string(self):
        length = self.seven_bit_int()
        value = self.data[self.pos:self.pos + length].decode("utf-8")
        self.pos += length
        return value


def read(path):
    reader = Reader(open(path, "rb").read())
    for _ in range(reader.seven_bit_int()):  # type reader manifest
        reader.string()
        reader.int32()
    reader.seven_bit_int()                   # shared resource count
    reader.seven_bit_int()                   # type of the primary object
    result = {}
    for _ in range(reader.uint32()):
        reader.seven_bit_int()
        key = reader.string()
        reader.seven_bit_int()
        result[key] = reader.string()
    return result


if __name__ == "__main__":
    json.dump(read(sys.argv[1]), open(sys.argv[2], "w", encoding="utf-8"),
              indent=1, ensure_ascii=False)
