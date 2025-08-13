# Employee Crud with store procedures 

Get - 

CREATE PROCEDURE proc_EmployeeGet  
AS  
    BEGIN  
        SELECT * FROM Employee   
        ORDER BY Id DESC  
    END


Post - 

CREATE PROCEDURE proc_EmployeePost  
@ID int,  
@EmployeeName varchar(50),  
@Salary decimal(18,2),  
@Email varchar(50),  
@DateOfBirth datetime,  
@UploadPicture varchar(max),  
@PayrollType int  
AS  
    BEGIN  
        INSERT INTO Employee (EmployeeName, Salary, Email, DateOfBirth, UploadPicture, PayrollType)  
        VALUES (@EmployeeName, @Salary, @Email, @DateOfBirth, @UploadPicture, @PayrollType)  
    END

Update - 

CREATE PROCEDURE proc_EmployeeUpdate  
@ID int,  
@EmployeeName varchar(50),  
@Salary decimal(18,2),  
@Email varchar(50),  
@DateOfBirth datetime,  
@UploadPicture varchar(max),  
@PayrollType int  
AS  
    BEGIN  
        UPDATE Employee   
        SET EmployeeName = @EmployeeName,  
            Salary = @Salary,  
            Email = @Email,  
            DateOfBirth = @DateOfBirth,  
            UploadPicture = @UploadPicture,  
            PayrollType = @PayrollType  
        WHERE Id = @ID  
    END

Delete - 

CREATE PROCEDURE proc_EmployeeDelete  
@ID int  
AS  
    BEGIN  
        DELETE FROM Employee   
        WHERE Id = @ID  
    END
