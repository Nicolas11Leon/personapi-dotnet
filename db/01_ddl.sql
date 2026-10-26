-- DDL: persona_db (SQL Server 2019/2022)
IF DB_ID('persona_db') IS NULL CREATE DATABASE persona_db;
GO
USE persona_db;
GO
IF OBJECT_ID('dbo.estudios') IS NOT NULL DROP TABLE dbo.estudios;
IF OBJECT_ID('dbo.telefono') IS NOT NULL DROP TABLE dbo.telefono;
IF OBJECT_ID('dbo.profesion') IS NOT NULL DROP TABLE dbo.profesion;
IF OBJECT_ID('dbo.persona') IS NOT NULL DROP TABLE dbo.persona;
GO
CREATE TABLE dbo.persona (
    cc       BIGINT      NOT NULL,
    nombre   VARCHAR(45) NOT NULL,
    apellido VARCHAR(45) NOT NULL,
    genero   CHAR(1)     NOT NULL,
    edad     INT         NULL,
    CONSTRAINT PK_persona PRIMARY KEY (cc),
    CONSTRAINT CK_persona_genero CHECK (genero IN ('M','F'))
);
CREATE TABLE dbo.profesion (
    id  INT          NOT NULL,
    nom VARCHAR(90)  NOT NULL,
    des TEXT         NULL,
    CONSTRAINT PK_profesion PRIMARY KEY (id)
);
CREATE TABLE dbo.telefono (
    num    VARCHAR(15) NOT NULL,
    oper   VARCHAR(45) NOT NULL,
    duenio BIGINT      NOT NULL,
    CONSTRAINT PK_telefono PRIMARY KEY (num),
    CONSTRAINT FK_telefono_persona FOREIGN KEY (duenio) REFERENCES dbo.persona(cc)
);
CREATE TABLE dbo.estudios (
    id_prof INT         NOT NULL,
    cc_per  BIGINT      NOT NULL,
    fecha   DATE        NULL,
    univer  VARCHAR(50) NULL,
    CONSTRAINT PK_estudios PRIMARY KEY (id_prof, cc_per),
    CONSTRAINT FK_estudio_profesion FOREIGN KEY (id_prof) REFERENCES dbo.profesion(id),
    CONSTRAINT FK_estudio_persona   FOREIGN KEY (cc_per)  REFERENCES dbo.persona(cc)
);
GO
