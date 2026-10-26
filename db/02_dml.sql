-- DML: datos de ejemplo
USE persona_db;
GO
INSERT INTO dbo.persona (cc, nombre, apellido, genero, edad) VALUES
 (1001, 'Ana',    'Gómez',   'F', 28),
 (1002, 'Luis',   'Pérez',   'M', 35),
 (1003, 'Marta',  'Rojas',   'F', 41),
 (1004, 'Carlos', 'Ramírez', 'M', 23);
INSERT INTO dbo.profesion (id, nom, des) VALUES
 (1, 'Ingeniería de Sistemas', 'Diseño y desarrollo de software'),
 (2, 'Diseño Gráfico',         'Comunicación visual'),
 (3, 'Administración',         'Gestión de organizaciones');
INSERT INTO dbo.telefono (num, oper, duenio) VALUES
 ('3001234567', 'Claro',   1001),
 ('3109876543', 'Movistar',1002),
 ('3201112233', 'Tigo',    1003),
 ('3157778899', 'Claro',   1001);
INSERT INTO dbo.estudios (id_prof, cc_per, fecha, univer) VALUES
 (1, 1001, '2020-12-10', 'Pontificia Universidad Javeriana'),
 (2, 1002, '2012-06-15', 'Universidad Nacional'),
 (3, 1003, '2005-11-30', 'Universidad de los Andes'),
 (1, 1004, '2024-06-01', 'Pontificia Universidad Javeriana');
GO
