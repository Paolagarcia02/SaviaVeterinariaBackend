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
    Photo_url VARCHAR(255), 
    Description TEXT, 
    Status ENUM('Con Dueño', 'En Adopción', 'Adoptado') NOT NULL DEFAULT 'Con Dueño',
    Owner_id INT NULL, 
    Created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_pets_owner FOREIGN KEY (Owner_id) REFERENCES User(User_id)
);

CREATE TABLE Appointment (
    Appointment_id INT AUTO_INCREMENT PRIMARY KEY,
    Date_time DATETIME NOT NULL,
    Duration_minutes INT DEFAULT 30, 
    Reason VARCHAR(100) NOT NULL,
    Status ENUM('Pendiente', 'Confirmada', 'Completada', 'Cancelada') DEFAULT 'Pendiente',
    Notes TEXT,
    Pet_id INT NOT NULL,
    Vet_id INT NOT NULL,
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
('Dr. House', 'house@savia.com', 'hashed_secret', 'Vet', 1),
('Alice Smith', 'alice@gmail.com', 'hashed_secret', 'User', NULL),
('Paola García', 'paola@gmail.com', 'hashed_secret', 'Admin', NULL);

INSERT INTO Pet (Name, Species, Breed, Status, Owner_id, Description) VALUES 
('Rex', 'Perro', 'Pastor Alemán', 'Con Dueño', 2, 'Un chico muy bueno.'),
('Luna', 'Gato', 'Persa', 'En Adopción', NULL, 'Busca hogar tranquilo. Le encanta dormir.');

INSERT INTO Clinic_room (Franchise_id, Name, Room_type) VALUES 
(1, 'Consulta 1', 'Consulta'),
(1, 'Consulta 2', 'Consulta'),
(1, 'Quirófano A', 'Quirófano'),
(1, 'Sala Rayos 1', 'Rayos X'),
(1, 'Lab General', 'Laboratorio');

INSERT INTO Appointment (Date_time, Reason, Pet_id, Vet_id, Franchise_id, Room_id, Status) VALUES 
('2024-03-01 10:00:00', 'Revisión Anual', 1, 1, 1, 1, 'Pendiente');

INSERT INTO Lab_test (Appointment_id, Test_type, Status) VALUES 
(1, 'Analítica Sanguínea', 'Solicitada');

INSERT INTO Adoption_application (User_id, Pet_id, Message) VALUES 
(2, 2, 'Me encantaría adoptar a Luna. Tengo experiencia con gatos y un hogar amoroso.');



