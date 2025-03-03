git add .
git commit -m "Your commit message"
git push origin <branch>


---------------------

INSERT INTO Departments (Name, Description)  
VALUES  
('Human Resources', 'Handles employee relations and administration.'),  
('Finance', 'Manages company financial transactions and budgets.'),  
('IT', 'Responsible for maintaining technology infrastructure.'),  
('Marketing', 'Handles branding, promotions, and advertising.'),  
('Sales', 'Focuses on client acquisition and revenue generation.'),  
('Operations', 'Oversees production and service delivery.'),  
('Customer Support', 'Provides assistance to customers and resolves issues.'),  
('Research and Development', 'Drives innovation and product improvement.');  


----------- CHECK IN DEPARTMENTS for DepartmentId --------------

INSERT INTO JobTitles (Name, Description, DepartmentId)  
VALUES  
('HR Manager', 'Oversees human resources policies and staff management.', 0),  
('Finance Analyst', 'Analyzes financial data and prepares reports.', 0),  
('Software Engineer', 'Develops and maintains software applications.', 0),  
('Marketing Specialist', 'Plans and executes marketing campaigns.', 0),  
('Sales Executive', 'Drives sales and builds client relationships.', 0),  
('Operations Manager', 'Ensures smooth day-to-day business operations.', 0),  
('Customer Support Representative', 'Assists customers with inquiries and issues.', 0),  
('R&D Engineer', 'Conducts research and develops new products.', 0);  

----------- RESET AUTO INCREMENT -------------

DBCC CHECKIDENT('tableName', RESEED, 0)
