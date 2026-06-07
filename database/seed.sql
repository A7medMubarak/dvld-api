-- ============================================================
-- DVLD-EFCore Seed Data
-- Run AFTER all migrations have been applied.
-- Usage: sqlcmd -S . -d DVLD-EFCore -i seed.sql
-- ============================================================

-- ============================================================
-- Step 1: Countries (no FK dependencies)
-- ============================================================
SET IDENTITY_INSERT Countries ON;
INSERT INTO Countries (CountryId, CountryName) VALUES
(1,   'Afghanistan'),
(2,   'Albania'),
(3,   'Algeria'),
(4,   'Andorra'),
(5,   'Angola'),
(6,   'Antigua and Barbuda'),
(7,   'Argentina'),
(8,   'Armenia'),
(9,   'Australia'),
(10,  'Austria'),
(11,  'Azerbaijan'),
(12,  'Bahamas'),
(13,  'Bahrain'),
(14,  'Bangladesh'),
(15,  'Barbados'),
(16,  'Belarus'),
(17,  'Belgium'),
(18,  'Belize'),
(19,  'Benin'),
(20,  'Bhutan'),
(21,  'Bolivia'),
(22,  'Bosnia and Herzegovina'),
(23,  'Botswana'),
(24,  'Brazil'),
(25,  'Brunei'),
(26,  'Bulgaria'),
(27,  'Burkina Faso'),
(28,  'Burundi'),
(29,  'Cabo Verde'),
(30,  'Cambodia'),
(31,  'Cameroon'),
(32,  'Canada'),
(33,  'Central African Republic'),
(34,  'Chad'),
(35,  'Chile'),
(36,  'China'),
(37,  'Colombia'),
(38,  'Comoros'),
(39,  'Congo'),
(40,  'Costa Rica'),
(41,  'Cote d''Ivoire'),
(42,  'Croatia'),
(43,  'Cuba'),
(44,  'Cyprus'),
(45,  'Czech Republic'),
(46,  'Denmark'),
(47,  'Djibouti'),
(48,  'Dominica'),
(49,  'Dominican Republic'),
(50,  'Ecuador'),
(51,  'Egypt'),
(52,  'El Salvador'),
(53,  'Equatorial Guinea'),
(54,  'Eritrea'),
(55,  'Estonia'),
(56,  'Eswatini'),
(57,  'Ethiopia'),
(58,  'Fiji'),
(59,  'Finland'),
(60,  'France'),
(61,  'Gabon'),
(62,  'Gambia'),
(63,  'Georgia'),
(64,  'Germany'),
(65,  'Ghana'),
(66,  'Greece'),
(67,  'Grenada'),
(68,  'Guatemala'),
(69,  'Guinea'),
(70,  'Guinea-Bissau'),
(71,  'Guyana'),
(72,  'Haiti'),
(73,  'Honduras'),
(74,  'Hungary'),
(75,  'Iceland'),
(76,  'India'),
(77,  'Indonesia'),
(78,  'Iran'),
(79,  'Iraq'),
(80,  'Ireland'),
(81,  'Israel'),
(82,  'Italy'),
(83,  'Jamaica'),
(84,  'Japan'),
(85,  'Jordan'),
(86,  'Kazakhstan'),
(87,  'Kenya'),
(88,  'Kiribati'),
(89,  'Kuwait'),
(90,  'Kyrgyzstan'),
(91,  'Laos'),
(92,  'Latvia'),
(93,  'Lebanon'),
(94,  'Lesotho'),
(95,  'Liberia'),
(96,  'Libya'),
(97,  'Liechtenstein'),
(98,  'Lithuania'),
(99,  'Luxembourg'),
(100, 'Madagascar'),
(101, 'Malawi'),
(102, 'Malaysia'),
(103, 'Maldives'),
(104, 'Mali'),
(105, 'Malta'),
(106, 'Marshall Islands'),
(107, 'Mauritania'),
(108, 'Mauritius'),
(109, 'Mexico'),
(110, 'Micronesia'),
(111, 'Moldova'),
(112, 'Monaco'),
(113, 'Mongolia'),
(114, 'Montenegro'),
(115, 'Morocco'),
(116, 'Mozambique'),
(117, 'Myanmar'),
(118, 'Namibia'),
(119, 'Nauru'),
(120, 'Nepal'),
(121, 'Netherlands'),
(122, 'New Zealand'),
(123, 'Nicaragua'),
(124, 'Niger'),
(125, 'Nigeria'),
(126, 'North Korea'),
(127, 'North Macedonia'),
(128, 'Norway'),
(129, 'Oman'),
(130, 'Pakistan'),
(131, 'Palau'),
(132, 'Palestine'),
(133, 'Panama'),
(134, 'Papua New Guinea'),
(135, 'Paraguay'),
(136, 'Peru'),
(137, 'Philippines'),
(138, 'Poland'),
(139, 'Portugal'),
(140, 'Qatar'),
(141, 'Romania'),
(142, 'Russia'),
(143, 'Rwanda'),
(144, 'Saint Kitts and Nevis'),
(145, 'Saint Lucia'),
(146, 'Saint Vincent and the Grenadines'),
(147, 'Samoa'),
(148, 'San Marino'),
(149, 'Sao Tome and Principe'),
(150, 'Saudi Arabia'),
(151, 'Senegal'),
(152, 'Serbia'),
(153, 'Seychelles'),
(154, 'Sierra Leone'),
(155, 'Singapore'),
(156, 'Slovakia'),
(157, 'Slovenia'),
(158, 'Solomon Islands'),
(159, 'Somalia'),
(160, 'South Africa'),
(161, 'South Korea'),
(162, 'South Sudan'),
(163, 'Spain'),
(164, 'Sri Lanka'),
(165, 'Sudan'),
(166, 'Suriname'),
(167, 'Sweden'),
(168, 'Switzerland'),
(169, 'Syria'),
(170, 'Taiwan'),
(171, 'Tajikistan'),
(172, 'Tanzania'),
(173, 'Thailand'),
(174, 'Timor-Leste'),
(175, 'Togo'),
(176, 'Tonga'),
(177, 'Trinidad and Tobago'),
(178, 'Tunisia'),
(179, 'Turkey'),
(180, 'Turkmenistan'),
(181, 'Tuvalu'),
(182, 'Uganda'),
(183, 'Ukraine'),
(184, 'United Arab Emirates'),
(185, 'United Kingdom'),
(186, 'United States of America'),
(187, 'Uruguay'),
(188, 'Uzbekistan'),
(189, 'Vanuatu'),
(190, 'Vatican City'),
(191, 'Venezuela'),
(192, 'Vietnam'),
(193, 'Yemen'),
(194, 'Zambia'),
(195, 'Zimbabwe');
SET IDENTITY_INSERT Countries OFF;
GO

-- ============================================================
-- Step 2: ApplicationTypes (must match enApplicationType enum)
-- ============================================================
SET IDENTITY_INSERT ApplicationTypes ON;
INSERT INTO ApplicationTypes (ApplicationTypeId, Title, Fees) VALUES
(1, 'New Driving License',               100.00),
(2, 'Renew Driving License',              50.00),
(3, 'Replace Lost Driving License',       20.00),
(4, 'Replace Damaged Driving License',    15.00),
(5, 'Release Detained Driving License',   50.00),
(6, 'New International License',         150.00),
(7, 'Retake Test',                        30.00);
SET IDENTITY_INSERT ApplicationTypes OFF;
GO

-- ============================================================
-- Step 3: LicenseClasses (must match enLicenseClasses enum)
-- ============================================================
SET IDENTITY_INSERT LicenseClasses ON;
INSERT INTO LicenseClasses (LicenseClassId, ClassName, ClassDescription, MinimumAllowedAge, DefaultValidityLength, ClassFees) VALUES
(1, 'Small Motorcycle',           'Motorcycles with engine capacity up to 125cc',           18, 10, 100.00),
(2, 'Heavy Motorcycle',           'Motorcycles with engine capacity over 125cc',            21, 10, 150.00),
(3, 'Ordinary Driving',           'Private cars and light vehicles',                        18, 10, 200.00),
(4, 'Commercial',                 'Taxis, buses, and commercial transport',                  21,  5, 300.00),
(5, 'Agricultural',               'Agricultural machinery and tractors',                     21, 10, 150.00),
(6, 'Small And Medium Bus',       'Buses with capacity up to 30 passengers',                21,  5, 250.00),
(7, 'Truck And Heavy Vehicle',    'Heavy trucks and articulated vehicles',                   24,  5, 400.00);
SET IDENTITY_INSERT LicenseClasses OFF;
GO

-- ============================================================
-- Step 4: TestTypes (must match enTestType enum)
-- ============================================================
SET IDENTITY_INSERT TestTypes ON;
INSERT INTO TestTypes (TestTypeId, Title, Description, Fees) VALUES
(1, 'Vision Test',  'Basic eyesight and color perception test',  20.00),
(2, 'Written Test', 'Traffic rules and road signs knowledge',   30.00),
(3, 'Street Test',  'On-road practical driving examination',     50.00);
SET IDENTITY_INSERT TestTypes OFF;
GO

-- ============================================================
-- Step 5: People (3 staff + 5 citizens)
-- Temporarily disable circular FK with Users
-- ============================================================
ALTER TABLE People NOCHECK CONSTRAINT FK_People_Users_CreatedByUserId;

SET IDENTITY_INSERT People ON;
INSERT INTO People (PersonId, NationalNo, FirstName, SecondName, ThirdName, LastName,
                    DateOfBirth, Address, Phone, Gender, Email,
                    NationalityCountryId, ImagePath, CreatedByUserId) VALUES
-- Staff (also Users)
(1, '29001011234567', 'Ahmed',  'Hassan',  'Mohamed', 'Ali',      '1990-01-01', 'Cairo, Egypt',     '01000000001', 0, 'ahmed.hassan@dvld.gov.eg',   51, NULL, 1),
(2, '29205151234567', 'Khaled', 'Omar',    'Sami',    'Abdullah', '1992-05-15', 'Alexandria, Egypt', '01000000002', 0, 'khaled.omar@dvld.gov.eg',     51, NULL, 2),
(3, '29508201234567', 'Samir',  'Mansour', 'Nabil',   'Youssef',  '1995-08-20', 'Giza, Egypt',       '01000000003', 0, 'samir.mansour@dvld.gov.eg',   51, NULL, 3),
-- Citizens (no User accounts -- added by Admin UserId=1)
(4, '29403081234568', 'Mona',   'Ibrahim', 'Hussein', 'El-Sayed', '1994-03-08', 'Cairo, Egypt',      '01000000004', 1, 'mona.ibrahim@example.com',    51, NULL, 1),
(5, '28811021234569', 'Youssef','Mahmoud', 'Ahmed',   'Farouk',   '1988-11-02', 'Cairo, Egypt',      '01000000005', 0, 'youssef.farouk@example.com',  51, NULL, 1),
(6, '29806251234570', 'Fatima', 'Ali',     'Hassan',  'Suleiman', '1998-06-25', 'Alexandria, Egypt',  '01000000006', 1, 'fatima.ali@example.com',      51, NULL, 1),
(7, '28509151234571', 'Karim',  'Adel',    'Sameh',   'Naguib',   '1985-09-15', 'Giza, Egypt',       '01000000007', 0, 'karim.naguib@example.com',    51, NULL, 1),
(8, '30004151234572', 'Nourhan','Tamer',   'Raafat',  'Galal',    '2000-04-15', 'Cairo, Egypt',      '01000000008', 1, 'nourhan.galal@example.com',   51, NULL, 1);
SET IDENTITY_INSERT People OFF;
GO

-- ============================================================
-- Step 6: Users (3 staff accounts only)
-- Password hashes pre-computed with BCrypt.Net-Next (work factor 11)
-- ============================================================
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, PersonId, UserName, PasswordHash, IsActive, Role) VALUES
(1, 1, 'admin',   '$2a$11$gsFww/O9zjvSfFF3gT5cTOyO3ki5u6x5v9zqzG9OPXXOwtkVeA9DC', 1, 'Admin'),
(2, 2, 'officer', '$2a$11$IayDviujo.2V1/a/lh8dS.fCzyDwqP6w/kC05buwgUCWF/dtE/4KS', 1, 'Officer'),
(3, 3, 'viewer',  '$2a$11$u2ifO.miP2Fw8SFY22Hj4ukrFkLEfPjv8YGsH9cO/kNC759MvdNiW', 1, 'Viewer');
SET IDENTITY_INSERT Users OFF;
GO

-- ============================================================
-- Step 7: Re-enable circular FK
-- ============================================================
ALTER TABLE People WITH CHECK CHECK CONSTRAINT FK_People_Users_CreatedByUserId;
GO

-- ============================================================
-- TRANSACTIONAL SCENARIO
-- Youssef Mahmoud (citizen, PersonId=5) applies for
-- Ordinary Driving License (Class 3). Ahmed (admin, UserId=1)
-- processes everything. Khaled (officer, UserId=2) detains.
-- ============================================================

-- Step 8: Application #1 — New Driving License for Youssef
SET IDENTITY_INSERT Applications ON;
INSERT INTO Applications (ApplicationId, ApplicantPersonId, ApplicationDate, ApplicationTypeId,
                          ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserId) VALUES
(1, 5, '2026-01-15', 1, 1, '2026-01-15', 100.00, 1);
SET IDENTITY_INSERT Applications OFF;
GO

-- Step 9: LocalDrivingLicenseApplication — Ordinary Driving (Class 3)
SET IDENTITY_INSERT LocalDrivingLicenseApplications ON;
INSERT INTO LocalDrivingLicenseApplications (LocalDrivingLicenseApplicationId, ApplicationId, LicenseClassId) VALUES
(1, 1, 3);
SET IDENTITY_INSERT LocalDrivingLicenseApplications OFF;
GO

-- Step 10: Driver record for Youssef
SET IDENTITY_INSERT Drivers ON;
INSERT INTO Drivers (DriverId, PersonId, CreatedByUserId, CreatedDate) VALUES
(1, 5, 1, '2026-01-25');
SET IDENTITY_INSERT Drivers OFF;
GO

-- Step 11: License issued — Ordinary Driving (Class 3), First Time (IssueReason=1)
SET IDENTITY_INSERT Licenses ON;
INSERT INTO Licenses (LicenseId, LicenseClassId, ApplicationId, DriverId,
                      IssueDate, ExpirationDate, IssueReason, Notes, PaidFees, IsActive, CreatedByUserId) VALUES
(1, 3, 1, 1, '2026-01-25', '2036-01-25', 1, NULL, 200.00, 1, 1);
SET IDENTITY_INSERT Licenses OFF;
GO

-- Step 12: Test Appointments — Vision → Written → Street (all locked after test taken)
SET IDENTITY_INSERT TestAppointments ON;
INSERT INTO TestAppointments (TestAppointmentId, TestTypeId, LocalDrivingLicenseApplicationId,
                              AppointmentDate, PaidFees, CreatedByUserId, IsLocked, RetakeTestApplicationId) VALUES
(1, 1, 1, '2026-01-16', 20.00, 1, 1, NULL),
(2, 2, 1, '2026-01-20', 30.00, 1, 1, NULL),
(3, 3, 1, '2026-01-25', 50.00, 1, 1, NULL);
SET IDENTITY_INSERT TestAppointments OFF;
GO

-- Step 13: Tests — all passed (TestResult=1)
SET IDENTITY_INSERT Tests ON;
INSERT INTO Tests (TestId, TestAppointmentId, TestResult, Notes, CreatedByUserId) VALUES
(1, 1, 1, 'Vision 20/20',           1),
(2, 2, 1, 'Written scored 48/50',   1),
(3, 3, 1, 'Street passed - no violations', 1);
SET IDENTITY_INSERT Tests OFF;
GO

-- Step 14: Complete the application (ApplicationStatus=3 = Completed)
UPDATE Applications SET ApplicationStatus = 3, LastStatusDate = '2026-01-25' WHERE ApplicationId = 1;
GO

-- Step 15: Detain Youssef's license (by Officer Khaled, UserId=2)
SET IDENTITY_INSERT DetainedLicenses ON;
INSERT INTO DetainedLicenses (DetainId, LicenseId, DetainDate, FineFees,
                              CreatedByUserId, IsReleased, ReleaseDate, ReleaseByUserId, ReleaseApplicationId) VALUES
(1, 1, '2026-02-01', 200.00, 2, 0, NULL, NULL, NULL);
SET IDENTITY_INSERT DetainedLicenses OFF;
GO

-- Step 16: Application #2 — Release Detained License (processed by Ahmed)
SET IDENTITY_INSERT Applications ON;
INSERT INTO Applications (ApplicationId, ApplicantPersonId, ApplicationDate, ApplicationTypeId,
                          ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserId) VALUES
(2, 5, '2026-02-05', 5, 3, '2026-02-05', 50.00, 1);
SET IDENTITY_INSERT Applications OFF;
GO

-- Step 17: Release the detained license
UPDATE DetainedLicenses SET
    IsReleased = 1,
    ReleaseDate = '2026-02-05',
    ReleaseByUserId = 1,
    ReleaseApplicationId = 2
WHERE DetainId = 1;
GO

-- ============================================================
-- Done
-- ============================================================
PRINT '============================================================';
PRINT 'DVLD-EFCore seed data inserted successfully.';
PRINT '============================================================';
PRINT '';
PRINT 'Test user credentials:';
PRINT '  admin   / [ask your team lead for dev credentials]';
PRINT '  officer / [ask your team lead for dev credentials]';
PRINT '  viewer  / [ask your team lead for dev credentials]';
PRINT '============================================================';
PRINT '';
PRINT 'Seed scenario:';
PRINT '  Youssef Mahmoud (citizen) applied for Ordinary Driving License';
PRINT '  Ahmed (admin) processed application, appointments, tests, license';
PRINT '  Youssef passed Vision, Written, Street tests (all passed)';
PRINT '  License #1 issued (valid 2026-01-25 to 2036-01-25)';
PRINT '  Khaled (officer) detained License #1 (fine: 200.00)';
PRINT '  Ahmed processed release application, License #1 released';
PRINT '============================================================';
GO
