# Valley Triad — Regras (documento de trabalho)

Um jogo de cartas no estilo Triple Triad (FF8) para o Stardew Valley. As cartas usam
**sprites do próprio jogo** (crops, aldeões, monstros, forrageáveis, criaturas lendárias).
Mod independente (veja o `ROADMAP.md` do repositório para o motivo de os minigames serem
lançados separadamente).

Este é um **documento vivo de design** — iteramos as regras aqui antes de escrever código.
Legenda: ✅ decidido · 🟡 proposto (falta confirmar) · ⏸ adiado · ❓ pergunta em aberto.

---

## 0. Diferenciação / prior art

Existe a mod **Pelican Packs** (Nexus 44098) — mas é de **outro gênero**: um simulador de
**abertura de pacotes/coleção** (comprar packs, abrir com animação, caçar raridades, vender
duplicatas, conquistas, questlines). **Não tem duelo, tabuleiro nem adversário**, e usa
**arte gerada por IA**.

O Valley Triad é um **jogo jogável** (duelo Triple Triad), então já se diferencia no núcleo.
Para **não** invadir o território dela:
- **Núcleo = jogar, não abrir pacotes.** Aquisição principal = **ganhar cartas vencendo
  NPCs** (baseado em habilidade), não gacha.
- **Diferencial de marketing:** **sprites reais do jogo, sem arte de IA.**
- **Evitar espelhar** o meta dela: comprar packs + vender duplicatas + conquistas +
  questline de coleção. Rebaixar pacotes a fonte secundária ou cortar do v1.
- ❓ (a checar) Existe algum clone direto de Triple Triad para SDV? Esse seria o concorrente
  real, não a Pelican Packs.

---

## 1. Regras centrais (base Triple Triad)

- ✅ 1 contra 1 vs. um NPC.
- ✅ Tabuleiro **3×3**.
- ✅ Cada jogador tem **5 cartas** por partida.
- ✅ Cada carta tem **4 valores de borda** (Norte / Leste / Sul / Oeste), de **1 a 10**,
  onde **10 é exibido como "A"**.
- ✅ Os jogadores se alternam colocando uma carta por turno numa casa vazia.
- ✅ **Captura:** ao colocar uma carta, para cada carta inimiga ortogonalmente adjacente,
  compara-se as duas bordas que se tocam; se a borda da carta recém-colocada for **maior**,
  a carta inimiga vira para a sua cor.
- ✅ **Fim da partida:** quando o tabuleiro está cheio (9 cartas colocadas). Vence quem tiver
  **mais cartas** (as do tabuleiro + a carta que sobrou na mão).
- ✅ **Empate ⇒ Morte Súbita (FF8).** No empate, joga-se de novo com a mão de cada jogador =
  as cartas que estavam sob o seu controle (na sua cor) no fim da partida empatada (cartas da
  sua cor no tabuleiro + a que sobrou na mão). Repete até alguém vencer, com teto de
  **N = 3** rounds; se ainda estiver empatado, é **empate de verdade**.

## 2. Regras avançadas ✅ (todas no lançamento)

- ✅ **Same (Igual):** se a carta colocada toca 2+ cartas em que as **bordas que se tocam são
  iguais**, todas essas cartas empatadas são capturadas (mesmo que a comparação normal não
  capturasse).
- ✅ **Plus (Soma):** se a carta colocada toca 2+ cartas em que as **somas** dos pares de
  bordas que se tocam são iguais, essas cartas são capturadas.
- ✅ **Combo:** cartas capturadas via Same/Plus disparam então capturas **normais** contra as
  próprias vizinhas, em cascata.
- ✅ **Same Wall** (a borda do tabuleiro conta como "A" para a regra Same): **fora do v1.**
  É nichada e confusa; pode virar um toggle depois.

## 3. Apostas / regra de troca

- ✅ **Regra de troca = "One":** o vencedor leva **uma** carta do perdedor.
- ✅ **Você só arrisca as 5 cartas que levou**, nunca a coleção inteira.
- ✅ **Ao vencer:** o jogador **escolhe** uma carta entre as 5 jogadas pelo NPC (satisfatório
  e alimenta a coleção).
- ✅ **Três modos de aposta (escalada de dor), selecionáveis no menu:**
  - **Amigável (padrão):** vencer dá uma carta; **perder não custa nada.**
  - **Difícil:** vencer dá uma carta; ao perder, você perde **uma carta aleatória** entre as
    5 que jogou.
  - **Ragnarök:** vencer dá uma carta; ao perder, o oponente leva a sua carta **mais valiosa**
    entre as 5 jogadas (a perda mais dolorosa possível — a IA escolhe).

## 4. Regra de elemento (provedor plugável)

O elemento é modelado como um provedor abstrato, para a fonte poder ser trocada/estendida.
**Elemento é só UMA etiqueta por carta e UMA por casa** — igual dá +1, diferente dá −1.
Não há relação *entre* as estações (não existe "Outono vence Inverno").

### Lançamento — Estações ✅
- O provedor entrega **Primavera / Verão / Outono / Inverno** (+ "Nenhum").
- Regra Elemental do FF8: uma casa do tabuleiro pode ter um elemento. Carta com elemento
  **igual** ao da casa ganha **+1 em todas as bordas**; carta de elemento **diferente ou sem
  elemento** numa casa elemental leva **−1 em todas as bordas**. (Aplicado nas comparações de
  captura.)
- ✅ O elemento de uma carta = a estação natural do tema (Abóbora = Outono, Morango =
  Primavera…). Temas **sem estação natural** (a maioria dos aldeões, monstros, minérios,
  artefatos) = **Nenhum** — sem atribuições arbitrárias. Isso torna as cartas de estação um
  pouco mais estratégicas.
- ✅ **Regra elemental LIGADA por padrão**, com **2–3 casas aleatórias** por partida recebendo
  um elemento de estação (+ toggle no menu para desligar a regra inteira).

### Expansão — Terrenos (locais do mapa) 📡 no radar (NÃO no v1)
Ideia guardada para depois: além do elemento de estação, uma casa poderia carregar um
**terreno = local do Vale** (Praia, Minas, Floresta, Cidade, Fazenda…) que dá **bônus a
cartas ligadas àquele local** (Praia turbina Willy/peixes; Minas turbina monstros/mineração;
Cidade turbina aldeões). É um segundo eixo temático de "elemento", por lugar em vez de
estação. **Fora do escopo do v1** — apenas manter no radar.

### Incremento — Espíritos do Elemental Force (⏸ ADIADO)
**Parkado até integrarmos de fato o Elemental Force.** Decisão de design até aqui: os
espíritos **não** substituem as estações e **não** são uma etiqueta de elemento alternativa
(estações continuam passivas; os espíritos teriam uma função própria). Papéis candidatos a
revisitar depois: (A) cartas de espírito com habilidade, (B) medidor de invocação, (C) duelos
de desafio/chefe com espíritos. Dependência opcional via reflection apenas
(`smapi-optional-deps`); ausente ⇒ no-op. Fora do escopo do v1.

## 5. Cartas & coleção

- ✅ Arte das cartas = sprites existentes no jogo (sem dependência de arte custom).
- ✅ **Cartas compostas em runtime (pacote minúsculo):** não embarcamos imagem de carta
  nenhuma. Ao desenhar, compõe-se ao vivo: **moldura** (1–2 texturas compartilhadas) +
  **sprite que o jogo já tem carregado** (crop/aldeão/monstro) + os **4 números** na fonte do
  jogo + **cor de raridade**. Cada carta é só uma **linha de dados** (sprite, bordas,
  raridade, estação, locais). Não é "gerar pixelart do zero" (isso ficaria feio); é
  **composição** de sprites reais — o que também reforça o "sem arte de IA" (§0).
- ✅ **Direção de arte: carta 100% pixel art, estilo fofo SDV.** Chassi de madeira em pixel
  com inlay/gemas por raridade + **cena procedural** na janela (céu + plantação/mina/mar +
  solo) servindo de "fundo pintado", e o **sprite herói destacado** (spotlight atrás +
  contorno escuro + sombra) para não se fundir ao fundo. Moedas de valor e selo de estação
  também em pixel. Valores N/L/S/O = **moedas nas bordas da janela**. Protótipo em
  `tools/render_pixel_card.py` (preview local `card-previews/pixel_*.png`).
- ✅ **Composição em runtime + cache `RenderTarget2D`** — só o chassi é asset fixo (1 textura);
  a cena por categoria é uma "receita" de sprites do jogo. Custo por frame = desenhar 1
  textura. Nenhuma arte de IA, nada de assets redistribuídos.
- ✅ **Cena com perspectiva** (estilo "campo" da referência): céu quente + linha de árvores +
  **canteiros procedurais** (touceiras verdes sobrepostas que recuam, com neblina de
  distância) + sulcos convergindo ao ponto de fuga. Enche a janela como um campo de verdade.
- ✅ **Fonte pixel própria** (bitmap 5×7 desenhado, com acentos pt-BR — Ó/Ã/Ê/Ç…) para
  números, nome e selo. Sem TTF suavizada. Números N/L/S/O nas moedas.
- ✅ **Cenas por categoria** (uma "receita" de fundo por tipo), prototipadas em
  `tools/render_pixel_card.py` — previews em `card-previews/scene_*.png`:
  campo (crop/forrageável) · pasto+cerca (animal) · mina+cristais (minério/monstro) ·
  mar+ondas (peixe) · Saloon+prateleira (aldeão) · noite estrelada (especial).
- ✅ **i18n do nome:** o nome é desenhado em runtime a partir de string **localizada**
  (`Item.DisplayName` do jogo p/ objetos; chaves i18n p/ o resto), nunca queimado numa
  imagem. Cache do `RenderTarget2D` **invalida no `Content.LocaleChanged`**. Fonte pixel
  cobre latim + acentos pt-BR; **fallback p/ `Game1.smallFont`** em scripts que ela não tem
  (CJK/ru), então funciona em todos os idiomas.
- 🟡 Folhagem dos canteiros ainda é genérica; dá para variar por cultivo depois. Template
  vetorial anterior (artifact) foi **descartado** em favor do pixel.
- 🟡 **Acervo de ~50 cartas no v1**, em 4 tiers de raridade:
  | Tier | Qtd. aprox. | Temas | Soma das bordas (4 lados, 1–10) |
  |---|---|---|---|
  | Comum | ~24 | crops, forrageáveis, animais de fazenda | 8–15 |
  | Incomum | ~15 | aldeões | 16–22 |
  | Raro | ~8 | monstros, itens artesanais, NPCs especiais | 23–29 |
  | Lendário | ~3–5 | Mago, Rei Junimo, Peixe Lendário, Sr. Qi | 30–36 (com alguns "A") |
- 🟡 **Duplicatas permitidas** (dá pra ter várias cópias de uma carta; necessário por causa
  dos pacotes + perdas no Ragnarök). A coleção guarda uma contagem por carta.
- ✅ **Montagem de deck:** antes da partida, escolha **5** da sua coleção, com tetos por
  raridade: **máx. 1 Lendária** e **máx. 2 Raras** por deck (evita decks "só bomba").
- ✅ **Aquisição (v1) — 100% baseado em jogar, sem gacha (nos distancia da Pelican Packs, §0):**
  1. **Pacote básico inicial entregue pela Abigail** (não há vendedor de pacotes). Ela
     apresenta o jogo num **evento inicial** (ver §6) — encaixa com a personagem gamer.
  2. Depois, **ganhar cartas vencendo NPCs** (regra de troca).
  Drops de monstro/pesca e qualquer venda de pacotes ficam **adiados**.

## 6. Adversários (NPCs)

- ✅ **Evento inicial (tutorial):** numa **sexta em que a Abigail está no Saloon**, ao
  entrar, o jogador dispara um evento em que a **Abigail apresenta o Valley Triad**, explica
  as regras e entrega o **pacote inicial**. (Detalhes do roteiro no `ROADMAP.md`.)
- ✅ **Entrada (v1) — sextas-feiras no Saloon:** o CTA do minigame fica habilitado só nas
  **sextas**, quando a turma toda está reunida no Saloon. Qualquer NPC presente pode ser
  desafiado; expandir para outros dias/locais em versões seguintes.
- ✅ **Quem pode ser desafiado:** todos, no fim das contas. Núcleo inicial da turma do
  Saloon: **Sam, Sebastian, Abigail, Gus, Leah, Elliott**, mais um "campeão" (**Sr. Qi** ou
  o **Mago**).
- ✅ **IA varia por NPC e por amizade:** base **gulosa** (maximiza capturas líquidas do turno,
  evita expor borda alta), mas cada NPC tem seu **perfil de IA** (uns jogam melhor que
  outros), e a força pode escalar com o nível de amizade. Lookahead mais esperto depois.
- ✅ **Decks escalam com a amizade:** mais corações ⇒ o NPC usa cartas mais fortes ⇒ cartas
  melhores para ganhar dele. Cada NPC tem um deck temático.
- ✅ **Recompensas:** a carta trocada + um pequeno ganho de amizade ao vencer. Uma questline
  de colecionador ("complete a coleção") é uma meta esticada para depois.

## Status de implementação (v0.1.0)

- ✅ Motor Triple Triad (`Game/Board.cs`): captura básica + Same/Plus/Combo + elemental.
- ✅ **Renderer** (`Rendering/CardRenderer.cs`): compõe a carta em runtime (chassi + cena por
  categoria + herói destacado + fonte pixel `PixelFont.cs`), cache em `RenderTarget2D`,
  invalida no idioma, herói via `ItemRegistry`/conteúdo do jogo, nome localizado.
- ✅ **Interação** (`UI/TriadMenu.cs`): partida jogável com animação de captura (flash),
  delay de turno do oponente, **apostas** (Amigável/Difícil/Ragnarök), **morte súbita** (teto
  configurável), **escolha de recompensa** na vitória, e callback de resultado.
- ✅ **Coleção + deck** (`Data/CollectionManager.cs`, `UI/DeckMenu.cs`): persistência por save
  (owned + deck), montagem com tetos (1 Lendária/2 Raras), auto-deck legal. Comando `vt_deck`.
- ✅ **Integração no mundo** (`ModEntry.cs`): evento inicial da **Abigail** (sexta no Saloon →
  pacote inicial), **desafiar NPCs** (ação sobre aldeão no Saloon nas sextas → partida),
  ganho/perda de carta + amizade conforme resultado.
- ✅ **IA/deck por amizade** (`Game/Opponents.cs`): tiers do deck e skill da IA escalam com os
  corações do NPC. Pacote inicial definido.
- ✅ **Roster completo**: 52 cartas geradas de `cards.xlsx` → `assets/cards.json` + i18n
  (EN/pt-BR) via `tools/gen_cards_json.py`. Sprites com **nomes reais de asset** (ex.:
  `Animal:White Chicken`, `Monster:Shadow Brute`, `Villager:MrQi`, `Char:Junimo`); placeholder
  "?" se algum faltar.
- ✅ **Desafio não conflita com presentear** (só com a mão vazia).
- ✅ **Empacotado**: `description.txt` (BBCode Nexus), `README.md`, `manifest` v1.0.0.
- 🟡 Falta só o **teste in-game** (o usuário fará). Polimentos opcionais futuros: animação de
  "virar" carta (hoje é flash), folhagem por cultivo, som/juice, balanceamento de recompensas.

## 7. Apresentação / UX — ⏭ discutir via wireframes

Esta seção será trabalhada com **wireframes** antes de definir. Telas a esboçar:
- Tabuleiro de partida (`IClickableMenu`): 3×3, mãos dos dois jogadores, casas com
  elemento/terreno, indicador de turno, legibilidade das cartas na resolução do SDV.
- Menu de coleção/álbum (ver e gerenciar cartas, contagem de duplicatas, filtros por tier).
- Tela de montagem de deck (escolher 5, respeitando tetos de raridade).
- Fluxo de entrada (CTA de sexta no Saloon) + tela de resultado/troca de carta.
- ✅ **Suporte só mouse + teclado** (mods não rodam em console, então controle não é
  prioridade).

---

## Registro de decisões / itens abertos

Decidido ✅:
- Regras centrais do Triple Triad · Same/Plus/Combo · Same Wall fora do v1.
- Regra de troca "One" · só as 5 cartas jogadas em risco · vitória = você escolhe entre as 5
  do NPC.
- **Três modos de aposta:** Amigável (sem perda) / Difícil (perde carta aleatória) / Ragnarök
  (perde a carta mais valiosa — a mais dolorosa).
- Morte Súbita (teto N=3 → empate).
- Estações = sistema de elemento do lançamento; temas sem estação = Nenhum; regra elemental
  LIGADA, 2–3 casas/partida (toggle no menu).
- Arte por **composição** de sprites do jogo (pacote minúsculo, sem arte de IA).
- Acervo ~50 em 4 tiers · duplicatas permitidas · montagem = escolher 5, **máx. 1 Lendária +
  máx. 2 Raras**.
- Aquisição v1 = **100% jogar**: pacote inicial entregue por um NPC + ganhar vencendo NPCs
  (sem vendedor/gacha).
- Entrada v1 = **sextas no Saloon**; todos desafiáveis; IA varia por NPC + amizade; decks
  escalam com amizade; recompensa = carta + amizade.

⏸ Adiado: espíritos do Elemental Force (até a integração do EF).

Suporte apenas **mouse + teclado** (sem controle — mods não rodam em console).
Terrenos (locais do mapa) 📡 no radar, fora do v1.

Tutorial/pacote inicial = **Abigail** (evento de sexta no Saloon).

**Acervo v1 preenchido** em `cards.xlsx` — 52 cartas reais do SDV (24 Comuns, 15 Incomuns,
8 Raras, 5 Lendárias). IDs de objeto **validados contra o `Content/Data/Objects.xnb` do jogo
instalado (v1.6.15)** — cada `(O)id` confere com o Name interno. Stat lines com passe de
balanceamento: força escala por tier e cada carta tem um "formato" (pico numa direção,
fraqueza na oposta); todas dentro do orçamento de borda.

**Em aberto 🟡/❓:**
- Iterar balanceamento fino após playtest (as stat lines são um 1º passe coerente).
- Toda a camada de **UX/menus** → via **wireframes** (§7).
