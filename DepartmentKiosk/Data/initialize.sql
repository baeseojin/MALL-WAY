CREATE TABLE Floors (
    Number INTEGER PRIMARY KEY,
    Name TEXT NOT NULL
);
CREATE TABLE Nodes (
    Id TEXT PRIMARY KEY,
    Floor INTEGER NOT NULL REFERENCES Floors(Number),
    X INTEGER NOT NULL,
    Y INTEGER NOT NULL,
    Kind TEXT NOT NULL CHECK (Kind IN ('corridor', 'kiosk', 'entrance', 'elevator', 'escalator')),
    Label TEXT NOT NULL
);
CREATE TABLE Places (
    EntranceNodeId TEXT PRIMARY KEY REFERENCES Nodes(Id),
    Name TEXT NOT NULL,
    Category TEXT NOT NULL,
    Area TEXT NOT NULL,
    Hours TEXT NOT NULL,
    X INTEGER NOT NULL,
    Y INTEGER NOT NULL,
    Width INTEGER NOT NULL CHECK (Width > 0),
    Height INTEGER NOT NULL CHECK (Height > 0)
);
CREATE TABLE Edges (
    FromNodeId TEXT NOT NULL REFERENCES Nodes(Id),
    ToNodeId TEXT NOT NULL REFERENCES Nodes(Id),
    Cost INTEGER NOT NULL CHECK (Cost > 0),
    Kind TEXT NOT NULL CHECK (Kind IN ('walk', 'elevator', 'escalator')),
    PRIMARY KEY (FromNodeId, ToNodeId),
    CHECK (FromNodeId <> ToNodeId)
);

INSERT INTO Floors VALUES
    (1, '푸드코트'),
    (2, '패션'),
    (3, '영화관');

INSERT INTO Nodes VALUES
    ('1-west', 1, 125, 235, 'corridor', '서쪽 복도'),
    ('1-center', 1, 340, 235, 'corridor', '중앙 복도'),
    ('1-east', 1, 555, 235, 'corridor', '동쪽 복도'),
    ('1-lift', 1, 240, 235, 'elevator', '중앙 엘리베이터'),
    ('1-stairs', 1, 440, 235, 'escalator', '동쪽 에스컬레이터'),
    ('1-kiosk', 1, 340, 296, 'kiosk', '1층 중앙 키오스크'),
    ('1-place-1', 1, 125, 170, 'entrance', '한끼마루 입구'),
    ('1-place-2', 1, 340, 170, 'entrance', '소담면가 입구'),
    ('1-place-3', 1, 555, 170, 'entrance', '구름다방 입구'),
    ('1-place-4', 1, 125, 330, 'entrance', '고객센터 입구'),
    ('1-place-5', 1, 340, 330, 'entrance', '화장실 입구'),
    ('1-place-6', 1, 555, 330, 'entrance', '주차장 연결 통로 입구'),
    ('2-west', 2, 125, 235, 'corridor', '서쪽 복도'),
    ('2-center', 2, 340, 235, 'corridor', '중앙 복도'),
    ('2-east', 2, 555, 235, 'corridor', '동쪽 복도'),
    ('2-lift', 2, 240, 235, 'elevator', '중앙 엘리베이터'),
    ('2-stairs', 2, 440, 235, 'escalator', '동쪽 에스컬레이터'),
    ('2-place-1', 2, 125, 170, 'entrance', '모노핏 입구'),
    ('2-place-2', 2, 340, 170, 'entrance', '온결웨어 입구'),
    ('2-place-3', 2, 555, 170, 'entrance', '리프스튜디오 입구'),
    ('2-place-4', 2, 125, 330, 'entrance', '스텝로그 입구'),
    ('2-place-5', 2, 340, 330, 'entrance', '수유실 입구'),
    ('2-place-6', 2, 555, 330, 'entrance', '화장실 입구'),
    ('3-west', 3, 125, 235, 'corridor', '서쪽 복도'),
    ('3-center', 3, 340, 235, 'corridor', '중앙 복도'),
    ('3-east', 3, 555, 235, 'corridor', '동쪽 복도'),
    ('3-lift', 3, 240, 235, 'elevator', '중앙 엘리베이터'),
    ('3-stairs', 3, 440, 235, 'escalator', '동쪽 에스컬레이터'),
    ('3-place-1', 3, 125, 170, 'entrance', '루미시네마 입구'),
    ('3-place-2', 3, 340, 170, 'entrance', '별빛스낵 입구'),
    ('3-place-3', 3, 555, 170, 'entrance', '화장실 입구');

INSERT INTO Places VALUES
    ('1-place-1', '한끼마루', '식당', '서쪽 구역', '10:30–21:00', 35, 55, 180, 115),
    ('1-place-2', '소담면가', '식당', '중앙 구역', '10:30–21:00', 250, 55, 180, 115),
    ('1-place-3', '구름다방', '카페', '동쪽 구역', '10:30–21:00', 465, 55, 180, 115),
    ('1-place-4', '고객센터', '편의시설', '서쪽 구역', '10:30–20:00', 35, 330, 180, 115),
    ('1-place-5', '화장실', '편의시설', '중앙 구역', '10:30–20:00', 250, 330, 180, 115),
    ('1-place-6', '주차장 연결 통로', '편의시설', '동쪽 구역', '10:30–20:00', 465, 330, 180, 115),
    ('2-place-1', '모노핏', '패션', '서쪽 구역', '10:30–21:00', 35, 55, 180, 115),
    ('2-place-2', '온결웨어', '패션', '중앙 구역', '10:30–21:00', 250, 55, 180, 115),
    ('2-place-3', '리프스튜디오', '패션', '동쪽 구역', '10:30–21:00', 465, 55, 180, 115),
    ('2-place-4', '스텝로그', '패션', '서쪽 구역', '10:30–21:00', 35, 330, 180, 115),
    ('2-place-5', '수유실', '편의시설', '중앙 구역', '10:30–20:00', 250, 330, 180, 115),
    ('2-place-6', '화장실', '편의시설', '동쪽 구역', '10:30–20:00', 465, 330, 180, 115),
    ('3-place-1', '루미시네마', '영화관', '서쪽 구역', '10:30–23:00', 35, 55, 180, 115),
    ('3-place-2', '별빛스낵', '식당', '중앙 구역', '10:30–21:00', 250, 55, 180, 115),
    ('3-place-3', '화장실', '편의시설', '동쪽 구역', '10:30–20:00', 465, 55, 180, 115);

-- Every connection represents travel in both directions.
INSERT INTO Edges VALUES
    ('1-west', '1-lift', 115, 'walk'),
    ('1-lift', '1-center', 100, 'walk'),
    ('1-center', '1-stairs', 100, 'walk'),
    ('1-stairs', '1-east', 115, 'walk'),
    ('1-center', '1-kiosk', 61, 'walk'),
    ('1-west', '1-place-1', 65, 'walk'),
    ('1-center', '1-place-2', 65, 'walk'),
    ('1-east', '1-place-3', 65, 'walk'),
    ('1-west', '1-place-4', 95, 'walk'),
    ('1-center', '1-place-5', 95, 'walk'),
    ('1-east', '1-place-6', 95, 'walk'),
    ('2-west', '2-lift', 115, 'walk'),
    ('2-lift', '2-center', 100, 'walk'),
    ('2-center', '2-stairs', 100, 'walk'),
    ('2-stairs', '2-east', 115, 'walk'),
    ('2-west', '2-place-1', 65, 'walk'),
    ('2-center', '2-place-2', 65, 'walk'),
    ('2-east', '2-place-3', 65, 'walk'),
    ('2-west', '2-place-4', 95, 'walk'),
    ('2-center', '2-place-5', 95, 'walk'),
    ('2-east', '2-place-6', 95, 'walk'),
    ('1-lift', '2-lift', 160, 'elevator'),
    ('1-stairs', '2-stairs', 105, 'escalator'),
    ('3-west', '3-lift', 115, 'walk'),
    ('3-lift', '3-center', 100, 'walk'),
    ('3-center', '3-stairs', 100, 'walk'),
    ('3-stairs', '3-east', 115, 'walk'),
    ('3-west', '3-place-1', 65, 'walk'),
    ('3-center', '3-place-2', 65, 'walk'),
    ('3-east', '3-place-3', 65, 'walk'),
    ('2-lift', '3-lift', 160, 'elevator'),
    ('2-stairs', '3-stairs', 105, 'escalator');
