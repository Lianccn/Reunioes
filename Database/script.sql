CREATE TABLE "Salas" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "Andar" INTEGER NOT NULL,
    "QuantidadeAssentos" INTEGER NOT NULL
);

CREATE TABLE "Reservas" (
    "Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    "SalaId" INTEGER NOT NULL,
    "Inicio" TEXT NOT NULL,
    "Fim" TEXT NOT NULL,
    CONSTRAINT "FK_Reservas_Salas_SalaId" FOREIGN KEY ("SalaId") REFERENCES "Salas" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Salas_Nome" ON "Salas" ("Nome");

CREATE INDEX "IX_Reservas_SalaId" ON "Reservas" ("SalaId");