SET NAMES utf8mb4;
SET CHARACTER SET utf8mb4;

DROP DATABASE IF EXISTS SaviaVetdb;
CREATE DATABASE SaviaVetdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE SaviaVetdb;

CREATE TABLE Franchise (
    Franchise_id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    Phone VARCHAR(20),
    Created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Service (
    Service_id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NOT NULL,
    Icon VARCHAR(50), 
    Created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Clinic_room (
    Room_id INT AUTO_INCREMENT PRIMARY KEY,
    Franchise_id INT NOT NULL, 
    Name VARCHAR(50) NOT NULL, 
    Room_type ENUM('Consulta', 'Quirófano', 'Rayos X', 'Urgencias', 'Laboratorio') NOT NULL,
    Is_active BOOLEAN DEFAULT TRUE, 
    CONSTRAINT fk_rooms_franchise FOREIGN KEY (Franchise_id) REFERENCES Franchise(Franchise_id)
);

CREATE TABLE Clinic_schedule (
    Schedule_id INT AUTO_INCREMENT PRIMARY KEY,
    Franchise_id INT NOT NULL,
    Room_id INT NULL,
    Day_of_week TINYINT NOT NULL, -- 0=Sunday ... 6=Saturday
    Open_time TIME NOT NULL,
    Close_time TIME NOT NULL,
    Is_open BOOLEAN DEFAULT TRUE,
    CONSTRAINT fk_schedule_franchise FOREIGN KEY (Franchise_id) REFERENCES Franchise(Franchise_id),
    CONSTRAINT fk_schedule_room FOREIGN KEY (Room_id) REFERENCES Clinic_room(Room_id)
);

CREATE TABLE User (
    User_id INT AUTO_INCREMENT PRIMARY KEY,
    Full_name VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password_hash VARCHAR(255) NOT NULL,
    Role ENUM('Admin', 'Vet', 'User') NOT NULL DEFAULT 'User',
    Franchise_id INT NULL, 
    Created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_users_franchise FOREIGN KEY (Franchise_id) REFERENCES Franchise(Franchise_id)
);

CREATE TABLE Pet (
    Pet_id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Species VARCHAR(50) NOT NULL, 
    Breed VARCHAR(50),
    Birth_date DATE,
    Photo_url VARCHAR(500), 
    Description TEXT, 
    Status ENUM('Con Dueño', 'En Adopción', 'Adoptado') NOT NULL DEFAULT 'Con Dueño',
    Owner_id INT NULL, 
    Created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_pets_owner FOREIGN KEY (Owner_id) REFERENCES User(User_id)
);

CREATE TABLE Appointment (
    Appointment_id INT AUTO_INCREMENT PRIMARY KEY,
    Date_time DATETIME NULL,
    Duration_minutes INT DEFAULT 30, 
    Reason VARCHAR(100) NOT NULL,
    Status ENUM('Pendiente', 'Confirmada', 'Completada', 'Cancelada') DEFAULT 'Pendiente',
    Notes TEXT,
    Pet_id INT NOT NULL,
    Vet_id INT NULL,
    Franchise_id INT NOT NULL,
    Room_id INT NULL, 
    CONSTRAINT fk_appointments_pet FOREIGN KEY (Pet_id) REFERENCES Pet(Pet_id),
    CONSTRAINT fk_appointments_vet FOREIGN KEY (Vet_id) REFERENCES User(User_id),
    CONSTRAINT fk_appointments_franchise FOREIGN KEY (Franchise_id) REFERENCES Franchise(Franchise_id),
    CONSTRAINT fk_appointments_room FOREIGN KEY (Room_id) REFERENCES Clinic_room(Room_id)
);

CREATE TABLE Lab_test (
    Test_id INT AUTO_INCREMENT PRIMARY KEY,
    Appointment_id INT NOT NULL,
    Test_type ENUM(
        'Analítica Sanguínea',   
        'Análisis de Orina',   
        'Coprocultivo',   
        'Radiografía',            
        'Ecografía',       
        'Biopsia',           
        'Citología',         
        'Otro'             
    ) NOT NULL DEFAULT 'Analítica Sanguínea',
    Result_data TEXT, 
    Comments VARCHAR(255), 
    Status ENUM('Solicitada', 'En Proceso', 'Completada') DEFAULT 'Solicitada',
    Requested_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    Completed_at DATETIME NULL,
    CONSTRAINT fk_lab_appointment FOREIGN KEY (Appointment_id) REFERENCES Appointment(Appointment_id)
);

CREATE TABLE Adoption_application (
    Application_id INT AUTO_INCREMENT PRIMARY KEY,
    User_id INT NOT NULL, 
    Pet_id INT NOT NULL, 
    Message TEXT, 
    Status ENUM('Pendiente', 'Aprobada', 'Rechazada') DEFAULT 'Pendiente',
    Application_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_app_user FOREIGN KEY (User_id) REFERENCES User(User_id),
    CONSTRAINT fk_app_pet FOREIGN KEY (Pet_id) REFERENCES Pet(Pet_id)
);

INSERT INTO Franchise (Name, Address, Phone) VALUES 
('Savia Central', 'Calle Principal 123', '555-0101'),
('Savia Norte', 'Avenida Norte 45', '555-0102'),
('Savia Sur', 'Polígono Sur 7', '555-0103');

INSERT INTO Service (Name, Description, Icon) VALUES 
('Medicina Preventiva', 'Vacunación, desparasitación y chequeos periódicos.', '💉'),
('Laboratorio Clínico', 'Análisis rápidos para un diagnóstico preciso.', '🔬'),
('Diagnóstico por Imagen', 'Radiología y ecografía de alta resolución.', '🦴'),
('Cirugía Avanzada', 'Quirófano equipado y monitorización segura.', '✂️'),
('Planes de Bienestar', 'Planes de salud anuales adaptados a tu mascota.', '❤️'),
('Urgencias', 'Atención prioritaria para casos críticos.', '🚨');

INSERT INTO User (Full_name, Email, Password_hash, Role, Franchise_id) VALUES 
('Vet', 'vet@savia.com', '1234', 'Vet', 1),
('User', 'user@gmail.com', '1234', 'User', 2),
('Admin', 'admin@savia.com', '1234', 'Admin', NULL),
('Marta Ruiz', 'marta.vet@savia.com', '1234', 'Vet', 1),
('Diego Serrano', 'diego.vet@savia.com', '1234', 'Vet', 2),
('Laura Pineda', 'laura.vet@savia.com', '1234', 'Vet', 3),
('Coordinacion Savia', 'coordinacion@savia.com', '1234', 'Admin', NULL),
('Ana Morales', 'ana.morales@gmail.com', '1234', 'User', 1),
('Carlos Pena', 'carlos.pena@gmail.com', '1234', 'User', 2),
('Sofia Gil', 'sofia.gil@gmail.com', '1234', 'User', 3),
('Marcos Leon', 'marcos.leon@gmail.com', '1234', 'User', 1),
('Lucia Nunez', 'lucia.nunez@gmail.com', '1234', 'User', 2),
('Pablo Rivas', 'pablo.rivas@gmail.com', '1234', 'User', 3),
('Irene Vega', 'irene.vega@gmail.com', '1234', 'User', 1),
('Jorge Cano', 'jorge.cano@gmail.com', '1234', 'User', 3),
('Elena Diaz', 'elena.diaz@gmail.com', '1234', 'User', 2),
('Raul Ortega', 'raul.ortega@gmail.com', '1234', 'User', 1);

INSERT INTO Pet (Name, Species, Breed, Birth_date, Photo_url, Description, Status, Owner_id) 
VALUES 
('Luna', 'Perro', 'Border Collie', '2022-05-15', 'https://images.unsplash.com/photo-1503256207526-0d5d80fa2f47?q=80&w=400', 'Luna es muy activa y le encanta correr tras el disco.', 'Con Dueño', 2),
('Simba', 'Gato', 'Común Europeo', '2023-01-10', 'https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?q=80&w=400', 'Un gato tranquilo que busca un sofá cálido y mucho cariño.', 'En Adopción', NULL),
('Rocky', 'Perro', 'Bulldog Francés', '2021-11-20', 'https://images.unsplash.com/photo-1583511655857-d19b40a7a54e?q=80&w=400', 'Pequeño pero con una gran personalidad. Se lleva bien con niños.', 'En Adopción', NULL),
('Nala', 'Gato', 'Siamés', '2023-06-01', 'https://images.unsplash.com/photo-1513245543132-31f507417b26?q=80&w=400', 'Nala fue adoptada recientemente y está en seguimiento veterinario.', 'Adoptado', 10),
('Coco', 'Perro', 'Labrador', '2020-03-12', 'https://images.unsplash.com/photo-1552053831-71594a27632d?q=80&w=400', 'El compañero ideal para largas caminatas por la montaña.', 'En Adopción', NULL),
('Toby', 'Perro', 'Beagle', '2019-09-21', 'https://images.unsplash.com/photo-1548199973-03cce0bbc87b?q=80&w=400', 'Le encanta rastrear olores y jugar en el parque.', 'Con Dueño', 8),
('Kira', 'Gato', 'Persa', '2021-04-03', 'https://images.unsplash.com/photo-1573865526739-10659fec78a5?q=80&w=400', 'Gata tranquila, requiere cepillado frecuente.', 'Con Dueño', 9),
('Max', 'Conejo', 'Belier', '2022-02-11', 'https://images.unsplash.com/photo-1585110396000-c9ffd4e4b308?q=80&w=400', 'Conejo sociable, acostumbrado a convivir con niños.', 'Con Dueño', 11),
('Mia', 'Perro', 'Mestizo', '2020-12-01', 'https://images.unsplash.com/photo-1543466835-00a7907e9de1?q=80&w=400', 'Rescatada, muy cariñosa y obediente.', 'Con Dueño', 12),
('Bruno', 'Gato', 'Maine Coon', '2018-07-19', 'https://images.unsplash.com/photo-1495360010541-f48722b34f7d?q=80&w=400', 'Necesita control de peso y revisión dental periódica.', 'Con Dueño', 14),
('Zeus', 'Perro', 'Pastor Alemán', '2017-10-05', 'https://images.unsplash.com/photo-1517849845537-4d257902454a?q=80&w=400', 'Perro senior con controles articulares trimestrales.', 'Con Dueño', 13),
('Nina', 'Perro', 'Caniche', '2024-01-08', 'https://images.unsplash.com/photo-1591160690555-5debfba289f0?q=80&w=400', 'Cachorra en calendario de vacunación.', 'Con Dueño', 16),
('Oliver', 'Gato', 'British Shorthair', '2022-08-27', 'https://images.unsplash.com/photo-1519052537078-e6302a4968d4?q=80&w=400', 'Gato muy glotón; está con dieta veterinaria.', 'Con Dueño', 17),
('Gala', 'Perro', 'Galgo', '2021-06-14', 'https://images.unsplash.com/photo-1450778869180-41d0601e046e?q=80&w=400', 'Muy dócil y tranquila. Ideal para piso.', 'En Adopción', NULL),
('Pixel', 'Gato', 'Común Europeo', '2023-09-30', 'https://images.unsplash.com/photo-1518791841217-8f162f1e1131?q=80&w=400', 'Activo y juguetón, convive bien con otros gatos.', 'En Adopción', NULL),
('Thor', 'Perro', 'Rottweiler', '2019-05-23', 'https://images.unsplash.com/photo-1568572933382-74d440642117?q=80&w=400', 'Se encuentra en buen estado general, control cardiaco anual.', 'Con Dueño', 10),
('Lila', 'Gato', 'Siamés', '2022-11-03', 'https://images.unsplash.com/photo-1533738363-b7f9aef128ce?q=80&w=400', 'Adoptada hace dos meses, en período de adaptación.', 'Adoptado', 8),
('Bongo', 'Hurón', 'Estándar', '2021-03-16', 'https://images.unsplash.com/photo-1583337130417-3346a1be7dee?q=80&w=400', 'Curioso y activo, requiere hogar con enriquecimiento ambiental.', 'En Adopción', NULL),
('Pelusa', 'Conejo', 'Toy', '2020-01-29', 'https://images.unsplash.com/photo-1425082661705-1834bfd09dca?q=80&w=400', 'Ya adoptada, continúa controles rutinarios.', 'Adoptado', 9),
('Rayo', 'Perro', 'Podenco', '2021-02-10', 'https://images.unsplash.com/photo-1537151625747-768eb6cf92b2?q=80&w=400', 'Muy enérgico, ideal para familia activa.', 'En Adopción', NULL);

INSERT INTO Clinic_room (Franchise_id, Name, Room_type) VALUES 
(1, 'Consulta 1', 'Consulta'),
(1, 'Consulta 2', 'Consulta'),
(1, 'Quirófano A', 'Quirófano'),
(1, 'Sala Rayos 1', 'Rayos X'),
(1, 'Lab General', 'Laboratorio'),
(2, 'Consulta 1', 'Consulta'),
(2, 'Consulta 2', 'Consulta'),
(2, 'Quirófano A', 'Quirófano'),
(2, 'Sala Rayos 1', 'Rayos X'),
(2, 'Lab General', 'Laboratorio'),
(3, 'Consulta 1', 'Consulta'),
(3, 'Consulta 2', 'Consulta'),
(3, 'Quirófano A', 'Quirófano'),
(3, 'Sala Rayos 1', 'Rayos X'),
(3, 'Lab General', 'Laboratorio');

INSERT INTO Clinic_schedule (Franchise_id, Room_id, Day_of_week, Open_time, Close_time, Is_open) VALUES
-- Horario general por franquicia (Lunes a Viernes 08:00-20:00; Sábado 09:00-14:00)
(1, NULL, 1, '08:00:00', '20:00:00', TRUE),
(1, NULL, 2, '08:00:00', '20:00:00', TRUE),
(1, NULL, 3, '08:00:00', '20:00:00', TRUE),
(1, NULL, 4, '08:00:00', '20:00:00', TRUE),
(1, NULL, 5, '08:00:00', '20:00:00', TRUE),
(1, NULL, 6, '09:00:00', '14:00:00', TRUE),
(2, NULL, 1, '08:00:00', '20:00:00', TRUE),
(2, NULL, 2, '08:00:00', '20:00:00', TRUE),
(2, NULL, 3, '08:00:00', '20:00:00', TRUE),
(2, NULL, 4, '08:00:00', '20:00:00', TRUE),
(2, NULL, 5, '08:00:00', '20:00:00', TRUE),
(2, NULL, 6, '09:00:00', '14:00:00', TRUE),
(3, NULL, 1, '08:00:00', '20:00:00', TRUE),
(3, NULL, 2, '08:00:00', '20:00:00', TRUE),
(3, NULL, 3, '08:00:00', '20:00:00', TRUE),
(3, NULL, 4, '08:00:00', '20:00:00', TRUE),
(3, NULL, 5, '08:00:00', '20:00:00', TRUE),
(3, NULL, 6, '09:00:00', '14:00:00', TRUE),
-- Ejemplo específico por sala (Quirófano A de cada franquicia, horario más corto)
(1, 3, 1, '09:00:00', '18:00:00', TRUE),
(1, 3, 2, '09:00:00', '18:00:00', TRUE),
(1, 3, 3, '09:00:00', '18:00:00', TRUE),
(1, 3, 4, '09:00:00', '18:00:00', TRUE),
(1, 3, 5, '09:00:00', '18:00:00', TRUE),
(2, 8, 1, '09:00:00', '18:00:00', TRUE),
(2, 8, 2, '09:00:00', '18:00:00', TRUE),
(2, 8, 3, '09:00:00', '18:00:00', TRUE),
(2, 8, 4, '09:00:00', '18:00:00', TRUE),
(2, 8, 5, '09:00:00', '18:00:00', TRUE),
(3, 13, 1, '09:00:00', '18:00:00', TRUE),
(3, 13, 2, '09:00:00', '18:00:00', TRUE),
(3, 13, 3, '09:00:00', '18:00:00', TRUE),
(3, 13, 4, '09:00:00', '18:00:00', TRUE),
(3, 13, 5, '09:00:00', '18:00:00', TRUE);

INSERT INTO Appointment (Date_time, Reason, Pet_id, Vet_id, Franchise_id, Room_id, Status) VALUES 
('2026-01-10 10:00:00', 'Vacunación anual', 1, 1, 1, 1, 'Confirmada'),
('2026-01-10 11:00:00', 'Control dermatológico', 6, 4, 1, 2, 'Completada'),
('2026-01-11 09:30:00', 'Esterilización programada', 7, 5, 2, 8, 'Confirmada'),
('2026-01-11 12:00:00', 'Cojera en pata trasera', 9, 5, 2, 6, 'Pendiente'),
('2026-01-12 16:00:00', 'Limpieza dental', 10, 4, 1, 3, 'Pendiente'),
('2026-01-13 10:15:00', 'Revisión senior', 11, 6, 3, 11, 'Confirmada'),
('2026-01-13 11:30:00', 'Analítica preoperatoria', 16, 6, 3, 15, 'Completada'),
('2026-01-14 09:00:00', 'Desparasitación', 12, 5, 2, 7, 'Completada'),
('2026-01-14 13:00:00', 'Dolor abdominal', 13, 1, 1, 5, 'Pendiente'),
('2026-01-15 17:00:00', 'Revisión postadopción', 17, 4, 1, 1, 'Confirmada'),
('2026-01-16 10:45:00', 'Evaluación para adopción', 2, 5, 2, 6, 'Pendiente'),
('2026-01-16 12:30:00', 'Control de peso', 8, 1, 1, 2, 'Cancelada'),
('2026-01-17 09:15:00', 'Herida superficial', 19, 5, 2, 9, 'Completada'),
('2026-01-17 11:00:00', 'Radiografía de tórax', 11, 6, 3, 14, 'Completada'),
('2026-01-18 15:00:00', 'Ecografía abdominal', 16, 6, 3, 15, 'Confirmada'),
('2026-01-19 10:00:00', 'Revisión general', 4, 6, 3, 12, 'Completada'),
('2026-01-20 18:00:00', 'Urgencia digestiva', 1, 1, 1, 4, 'Confirmada'),
('2026-01-21 09:00:00', 'Control postquirúrgico', 7, 5, 2, 7, 'Confirmada');

INSERT INTO Lab_test (Appointment_id, Test_type, Result_data, Comments, Status, Completed_at) VALUES 
(3, 'Analítica Sanguínea', 'Parámetros prequirúrgicos dentro de rango.', 'Apta para cirugía.', 'Completada', '2026-01-11 08:45:00'),
(7, 'Análisis de Orina', 'Sin signos de infección urinaria.', 'Control correcto.', 'Completada', '2026-01-13 12:10:00'),
(9, 'Coprocultivo', NULL, 'Pendiente de resultado microbiológico.', 'En Proceso', NULL),
(14, 'Radiografía', 'Sin lesiones pulmonares evidentes.', 'Leve inflamación bronquial.', 'Completada', '2026-01-17 11:40:00'),
(15, 'Ecografía', NULL, 'Prueba solicitada por dolor abdominal recurrente.', 'Solicitada', NULL),
(17, 'Analítica Sanguínea', NULL, 'Control de hidratación y electrolitos.', 'En Proceso', NULL),
(13, 'Citología', 'Muestra compatible con inflamación leve.', 'No se observan células malignas.', 'Completada', '2026-01-17 10:05:00'),
(4, 'Analítica Sanguínea', NULL, 'Previa a valoración traumatológica.', 'Solicitada', NULL),
(6, 'Biopsia', NULL, 'Se programa toma de muestra en siguiente visita.', 'Solicitada', NULL),
(18, 'Análisis de Orina', NULL, 'Control posterior a cirugía.', 'Solicitada', NULL);

INSERT INTO Adoption_application (User_id, Pet_id, Message, Status) VALUES 
(8, 2, 'Vivo en un piso tranquilo y teletrabajo, tendría mucho tiempo para Simba.', 'Pendiente'),
(9, 3, 'Tengo experiencia con perros braquicéfalos y seguimiento veterinario continuo.', 'Pendiente'),
(10, 14, 'Busco un galgo adulto para convivencia familiar en casa con jardín.', 'Pendiente'),
(11, 15, 'Me gustaría adoptar a Pixel y puedo asumir los cuidados diarios.', 'Rechazada'),
(12, 18, 'He tenido hurones antes y dispongo de espacio adaptado.', 'Pendiente'),
(13, 20, 'Hago deporte a diario y Rayo tendría paseos largos.', 'Pendiente'),
(14, 5, 'Estoy interesado en Coco y ya tengo experiencia con labradores.', 'Pendiente'),
(15, 3, 'Podría ofrecerle un hogar estable y visitas regulares al veterinario.', 'Pendiente'),
(16, 2, 'Conozco bien el comportamiento felino y su adaptación.', 'Rechazada'),
(17, 14, 'Quiero adoptar a Gala y cuento con casa amplia y segura.', 'Pendiente'),
(2, 20, 'Me interesa Rayo, tengo tiempo y energía para su actividad.', 'Pendiente'),
(9, 19, 'Adopción finalizada tras período de adaptación satisfactorio.', 'Aprobada'),
(8, 17, 'Adopción cerrada con seguimiento de conducta positivo.', 'Aprobada'),
(10, 4, 'Nala se adaptó muy bien al hogar durante la preadopción.', 'Aprobada');
