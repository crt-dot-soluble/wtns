@echo off

REM CREATE THE DATABASE
mysql -uroot -proot -e "CREATE DATABASE IF NOT EXISTS WTNS;"


REM CREATE THE USERS TABLE
mysql -uroot -proot -e "USE WTNS; CREATE TABLE IF NOT EXISTS Users (ID INT AUTO_INCREMENT PRIMARY KEY, UserName VARCHAR(32) UNIQUE NOT NULL, DisplayName VARCHAR(32), Bio VARCHAR(512), Hash CHAR(60) NOT NULL, Salt CHAR(32) NOT NULL, Active BIT NOT NULL DEFAULT 1); DESCRIBE Users;"


REM THE OPERATION SHOULD CREATE A TABLE WITH THE FOLLOWING STRUCTURE
REM +-------------+--------------+------+-----+---------+----------------+
REM | Field       | Type         | Null | Key | Default | Extra          |
REM +-------------+--------------+------+-----+---------+----------------+
REM | ID          | int          | NO   | PRI | NULL    | auto_increment |
REM | UserName    | varchar(32)  | NO   | UNI | NULL    |                |
REM | DisplayName | varchar(32)  | YES  |     | NULL    |                |
REM | Bio         | varchar(512) | YES  |     | NULL    |                |
REM | Hash        | char(60)     | NO   |     | NULL    |                |
REM +-------------+--------------+------+-----+---------+----------------+


REM CREATE THE POSTS TABLE
mysql -uroot -proot -e "USE WTNS; CREATE TABLE IF NOT EXISTS Posts (PostID int NOT NULL AUTO_INCREMENT, UserID int NOT NULL, PostContent varchar(255) NOT NULL, PostDate TIMESTAMP DEFAULT CURRENT_TIMESTAMP, PRIMARY KEY (PostID), FOREIGN KEY (UserID) REFERENCES Users(ID));"

