-- PostgreSQL initialization script
-- This script will be executed when the container starts

-- Create tables if they don't exist
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" serial PRIMARY KEY,
    "Email" varchar(256) NOT NULL UNIQUE,
    "PasswordHash" text NOT NULL,
    "Role" varchar(50) NOT NULL DEFAULT 'User'
);

CREATE TABLE IF NOT EXISTS "Tasks" (
    "Id" serial PRIMARY KEY,
    "Title" varchar(200) NOT NULL,
    "Description" text,
    "Status" varchar(50) NOT NULL,
    "Position" integer NOT NULL,
    "UserId" integer NOT NULL,
    "UserName" varchar(256) NOT NULL,
    "AssignedToUserId" integer,
    "AssignedToUserName" varchar(256),
    "DueDate" timestamp,
    "Priority" varchar(50) NOT NULL,
    "Tags" text,
    "Comments" text,
    "CreatedAt" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS "TaskHistories" (
    "Id" serial PRIMARY KEY,
    "TaskId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "UserName" varchar(256) NOT NULL,
    "Action" varchar(100) NOT NULL,
    "OldValue" text,
    "NewValue" text,
    "Details" text,
    "CreatedAt" timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS "RefreshTokens" (
    "Id" serial PRIMARY KEY,
    "Token" varchar(500) NOT NULL UNIQUE,
    "UserId" integer NOT NULL,
    "ExpiresAt" timestamp NOT NULL,
    "IsRevoked" boolean NOT NULL DEFAULT false
);

-- Insert default data if tables are empty
INSERT INTO "Users" ("Email", "PasswordHash", "Role")
SELECT 'admin@test.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Admin'
WHERE NOT EXISTS (SELECT 1 FROM "Users" WHERE "Email" = 'admin@test.com');

INSERT INTO "Users" ("Email", "PasswordHash", "Role")
SELECT 'user@test.com', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'User'
WHERE NOT EXISTS (SELECT 1 FROM "Users" WHERE "Email" = 'user@test.com');

INSERT INTO "Users" ("Id", "Email", "PasswordHash", "Role")
SELECT 999, 'doc_js_galindo@fesc.edu.co', '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Admin'
WHERE NOT EXISTS (SELECT 1 FROM "Users" WHERE "Id" = 999);

-- Add foreign key constraints if they don't exist
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Tasks_Users_UserId') THEN
        ALTER TABLE "Tasks" ADD CONSTRAINT "FK_Tasks_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id");
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_Tasks_Users_AssignedToUserId') THEN
        ALTER TABLE "Tasks" ADD CONSTRAINT "FK_Tasks_Users_AssignedToUserId" FOREIGN KEY ("AssignedToUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_TaskHistories_Users_UserId') THEN
        ALTER TABLE "TaskHistories" ADD CONSTRAINT "FK_TaskHistories_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id");
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_TaskHistories_Tasks_TaskId') THEN
        ALTER TABLE "TaskHistories" ADD CONSTRAINT "FK_TaskHistories_Tasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'FK_RefreshTokens_Users_UserId') THEN
        ALTER TABLE "RefreshTokens" ADD CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE;
    END IF;
END $$;