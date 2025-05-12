-- =============================================
-- Create Table: todo_list
-- =============================================
CREATE TABLE IF NOT EXISTS todo_list (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255),
    colour VARCHAR(7) NOT NULL DEFAULT '#FFFFFF',
    
    created TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    last_modified TIMESTAMPTZ,
    last_modified_by VARCHAR(100)
);

-- =============================================
-- Create Table: todo_item
-- =============================================
CREATE TABLE IF NOT EXISTS todo_item (
    id SERIAL PRIMARY KEY,
    list_id INT REFERENCES todo_list(id) ON DELETE CASCADE,
    title VARCHAR(255),
    note TEXT,
    priority INT NOT NULL DEFAULT 0,
    reminder TIMESTAMPTZ,
    
    done BOOLEAN NOT NULL DEFAULT FALSE,

    created TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    last_modified TIMESTAMPTZ,
    last_modified_by VARCHAR(100)
);

-- =============================================
-- Create Table: user_role
-- =============================================
CREATE TABLE IF NOT EXISTS user_role (
    id SERIAL PRIMARY KEY,
    user_id VARCHAR(100),
    role VARCHAR(100),
    
    created TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    last_modified TIMESTAMPTZ,
    last_modified_by VARCHAR(100)
);

-- =============================================
-- Create Table: domain_event
-- =============================================
CREATE TABLE IF NOT EXISTS domain_event (
    id SERIAL PRIMARY KEY,
    event_id UUID NOT NULL,
    event_type VARCHAR(255) NOT NULL,
    event_data TEXT NOT NULL,
    status VARCHAR(50) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    processed_at TIMESTAMPTZ NULL,
    result VARCHAR(255) NULL,

    created TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by VARCHAR(100),
    last_modified TIMESTAMPTZ,
    last_modified_by VARCHAR(100)
);

-- =============================================
-- Insert Default user_role if not exists
-- =============================================
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM user_role 
        WHERE user_id = 'i53MEl0y7jOwIh0BvmHqd0PMDnf2' 
        AND role = 'Administrator'
    ) THEN
        INSERT INTO user_role (user_id, role, created, created_by) 
        VALUES ('i53MEl0y7jOwIh0BvmHqd0PMDnf2', 'Administrator', NOW(), 'SYSTEM');
    END IF;
END $$;
