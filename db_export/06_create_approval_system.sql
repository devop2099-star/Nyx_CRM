-- ============================================================================
-- NYX APPROVAL ENGINE - GENERIC DB SCHEMA AND INITIAL SEED DATA
-- ============================================================================

BEGIN;

CREATE SCHEMA IF NOT EXISTS approval_service;

-- 1. Catálogo maestro de reglas de aprobación por etapa/departamento
CREATE TABLE IF NOT EXISTS approval_service.approval_rule_catalog (
    id_approval_catalog SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    applies_to VARCHAR(100) NOT NULL DEFAULT 'Toda campaña nueva',
    trigger_stage_id INTEGER NOT NULL,
    required_department VARCHAR(100) NOT NULL,
    is_mandatory BOOLEAN NOT NULL DEFAULT TRUE,
    blocks_progress BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_commission BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_activation BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_liquidation BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE approval_service.approval_rule_catalog OWNER TO ronald;

-- 2. Solicitud principal de aprobación por entidad (Decoupled: entity_type + entity_id)
CREATE TABLE IF NOT EXISTS approval_service.entity_approval_request (
    id_approval_request SERIAL PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL, -- "Order", "Lead", "Budget", etc.
    entity_id VARCHAR(50) NOT NULL, -- Identificador único en el sistema cliente
    trigger_stage_id INTEGER NOT NULL,
    overall_status VARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, APPROVED, REJECTED
    requested_by INTEGER,
    reason TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE approval_service.entity_approval_request OWNER TO ronald;
CREATE INDEX IF NOT EXISTS idx_approval_req_lookup ON approval_service.entity_approval_request(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_approval_req_status ON approval_service.entity_approval_request(overall_status);

-- 3. Ítems de firma individual por departamento/rol
CREATE TABLE IF NOT EXISTS approval_service.entity_approval_item (
    id_approval_item SERIAL PRIMARY KEY,
    id_approval_request INTEGER NOT NULL REFERENCES approval_service.entity_approval_request(id_approval_request) ON DELETE CASCADE,
    id_approval_catalog INTEGER REFERENCES approval_service.approval_rule_catalog(id_approval_catalog) ON DELETE SET NULL,
    department VARCHAR(100) NOT NULL,
    is_mandatory BOOLEAN NOT NULL DEFAULT TRUE,
    blocks_progress BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_commission BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_activation BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_liquidation BOOLEAN NOT NULL DEFAULT FALSE,
    status VARCHAR(50) NOT NULL DEFAULT 'PENDING', -- PENDING, APPROVED, REJECTED
    comments TEXT,
    resolved_by INTEGER,
    resolved_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE approval_service.entity_approval_item OWNER TO ronald;
CREATE INDEX IF NOT EXISTS idx_approval_item_request ON approval_service.entity_approval_item(id_approval_request);

-- ============================================================================
-- SEEDING CATALOG RULES (approval_service)
-- ============================================================================

TRUNCATE approval_service.entity_approval_item CASCADE;
TRUNCATE approval_service.entity_approval_request CASCADE;
TRUNCATE approval_service.approval_rule_catalog CASCADE;

-- Regla 1: Data & Analytics (Estructura - siempre obligatoria)
INSERT INTO approval_service.approval_rule_catalog 
(id_approval_catalog, name, applies_to, trigger_stage_id, required_department, is_mandatory, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation)
VALUES (1, 'Aprobación Data & Analytics', 'Toda campaña nueva', 3, 'Data & Analytics', TRUE, FALSE, FALSE, FALSE, FALSE);

-- Regla 2: Finanzas (Negocio - dueño del proceso)
INSERT INTO approval_service.approval_rule_catalog 
(id_approval_catalog, name, applies_to, trigger_stage_id, required_department, is_mandatory, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation)
VALUES (2, 'Aprobación Finanzas', 'Toda campaña nueva', 3, 'Finanzas', TRUE, FALSE, TRUE, FALSE, TRUE);

-- Regla 3: Gerencia (Obligatoria si bloquea avance)
INSERT INTO approval_service.approval_rule_catalog 
(id_approval_catalog, name, applies_to, trigger_stage_id, required_department, is_mandatory, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation)
VALUES (3, 'Aprobación Gerencia', 'Toda campaña nueva', 3, 'Gerencia', TRUE, TRUE, TRUE, TRUE, TRUE);

COMMIT;
