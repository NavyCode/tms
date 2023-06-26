
2. create database testitdb;
create user tester with encrypted password 'tester';
grant all privileges on database testitdb to tester;
\connect testitdb;

docker build -t testplan .

2. "features": { "buildkit": false }
