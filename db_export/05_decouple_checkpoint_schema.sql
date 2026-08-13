-- ============================================================================
-- NYX CHECKPOINT ENGINE - DECOUPLED GENERIC SCHEMA AND SEED DATA
-- ============================================================================

BEGIN;

CREATE SCHEMA IF NOT EXISTS checkpoint_service;

-- 1. Catálogo maestro de checkpoints (Reglas genéricas)
CREATE TABLE IF NOT EXISTS checkpoint_service.checkpoint_catalog (
    id_checkpoint_catalog SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    applies_to VARCHAR(100) NOT NULL, -- Filtro de metadatos (ej: nombre de campaña)
    trigger_stage_id INTEGER NOT NULL, -- Identificador de la etapa en el cliente
    origin VARCHAR(50) NOT NULL DEFAULT 'INTERNO',
    scope VARCHAR(50) NOT NULL DEFAULT 'Venta',
    blocks_progress BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_commission BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_activation BOOLEAN NOT NULL DEFAULT FALSE,
    blocks_liquidation BOOLEAN NOT NULL DEFAULT FALSE,
    rollback_stage_id INTEGER, -- Etapa de retroceso sugerida en el cliente
    trigger_on_ko_catalog_id INTEGER REFERENCES checkpoint_service.checkpoint_catalog(id_checkpoint_catalog) ON DELETE SET NULL,
    owner_department VARCHAR(100) NOT NULL,
    required_steps_count INTEGER NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE checkpoint_service.checkpoint_catalog OWNER TO ronald;

-- 2. Checklist o Pasos de cada plantilla
CREATE TABLE IF NOT EXISTS checkpoint_service.checkpoint_step (
    id_checkpoint_step SERIAL PRIMARY KEY,
    id_checkpoint_catalog INTEGER NOT NULL REFERENCES checkpoint_service.checkpoint_catalog(id_checkpoint_catalog) ON DELETE CASCADE,
    step_index INTEGER NOT NULL,
    description VARCHAR(255) NOT NULL,
    UNIQUE (id_checkpoint_catalog, step_index)
);

ALTER TABLE checkpoint_service.checkpoint_step OWNER TO ronald;

-- 3. Instancias de Checkpoint por Entidad (Decoupled: entity_type + entity_id)
CREATE TABLE IF NOT EXISTS checkpoint_service.entity_checkpoint (
    id_entity_checkpoint SERIAL PRIMARY KEY,
    entity_type VARCHAR(50) NOT NULL, -- "Order", "Lead", etc.
    entity_id VARCHAR(50) NOT NULL, -- Identificador único en el cliente
    id_checkpoint_catalog INTEGER NOT NULL REFERENCES checkpoint_service.checkpoint_catalog(id_checkpoint_catalog) ON DELETE RESTRICT,
    status VARCHAR(50) NOT NULL DEFAULT 'PENDIENTE', -- PENDIENTE, EN_PROCESO, APROBADO, RECHAZADO, KO
    comments TEXT,
    checked_by INTEGER,
    checked_at TIMESTAMP WITH TIME ZONE,
    steps_completed INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE checkpoint_service.entity_checkpoint OWNER TO ronald;
CREATE INDEX IF NOT EXISTS idx_entity_checkpoint_lookup ON checkpoint_service.entity_checkpoint(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_entity_checkpoint_status ON checkpoint_service.entity_checkpoint(status);

-- 4. Estado de pasos individuales
CREATE TABLE IF NOT EXISTS checkpoint_service.entity_checkpoint_step_status (
    id_entity_checkpoint INTEGER NOT NULL REFERENCES checkpoint_service.entity_checkpoint(id_entity_checkpoint) ON DELETE CASCADE,
    step_index INTEGER NOT NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    updated_by INTEGER,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_entity_checkpoint, step_index)
);

ALTER TABLE checkpoint_service.entity_checkpoint_step_status OWNER TO ronald;

-- ============================================================================
-- SEEDING CATALOG DATA (checkpoint_service)
-- ============================================================================

TRUNCATE checkpoint_service.checkpoint_step CASCADE;
TRUNCATE checkpoint_service.checkpoint_catalog CASCADE;

-- 1. Auditoría de audio
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (1, 'Auditoría de audio', 'Alarmas', 9, 'INTERNO', 'Venta', FALSE, TRUE, TRUE, FALSE, NULL, NULL, 'Finanzas', 5);

INSERT INTO checkpoint_service.checkpoint_step (id_checkpoint_catalog, step_index, description) VALUES
(1, 1, 'Verificación de la nitidez de la voz del titular'),
(1, 2, 'Confirmación del consentimiento explícito de contratación'),
(1, 3, 'Validación de los datos bancarios dictados'),
(1, 4, 'Verificación de lectura obligatoria del RGPD y condiciones de uso'),
(1, 5, 'Confirmación de coincidencia de DNI/NIE expresado verbalmente');

-- 2. Revisión de supervisor
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (2, 'Revisión de supervisor', 'Toda campaña nueva', 2, 'INTERNO', 'Venta', TRUE, FALSE, FALSE, FALSE, NULL, NULL, 'Operaciones', 0);

-- 3. Confirmación de firma de contrato
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (3, 'Confirmación de firma de contrato', 'Toda campaña nueva', 8, 'PROVEEDOR', 'Venta', TRUE, TRUE, FALSE, TRUE, NULL, NULL, 'Backoffice', 0);

-- 4. Seguimiento de satisfacción postventa
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (4, 'Seguimiento de satisfacción postventa', 'Toda campaña nueva', 9, 'INTERNO', 'Venta', FALSE, FALSE, FALSE, FALSE, NULL, NULL, 'Postventa', 3);

INSERT INTO checkpoint_service.checkpoint_step (id_checkpoint_catalog, step_index, description) VALUES
(4, 1, 'Llamada de bienvenida realizada con éxito'),
(4, 2, 'Confirmación de instalación correcta de equipos/servicios'),
(4, 3, 'Validación del índice NPS inicial favorable');

-- 5. Validación de Backoffice
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (5, 'Validación de Backoffice', 'Toda campaña nueva', 3, 'INTERNO', 'Venta', TRUE, FALSE, FALSE, FALSE, 2, NULL, 'Backoffice', 0);

-- 6. Validación de documentos externos
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (6, 'Validación de documentos externos', 'Toda campaña nueva', 4, 'INTERNO', 'Venta', TRUE, FALSE, FALSE, FALSE, 5, NULL, 'Backoffice', 0);

-- 7. Gestión de recuperación
INSERT INTO checkpoint_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (7, 'Gestión de recuperación', 'Toda campaña nueva', 9, 'INTERNO', 'Venta', FALSE, TRUE, FALSE, FALSE, NULL, 4, 'Postventa', 3);

INSERT INTO checkpoint_service.checkpoint_step (id_checkpoint_catalog, step_index, description) VALUES
(7, 1, 'Intento de contacto telefónico registrado'),
(7, 2, 'Verificación de motivo de impago o incidencia técnica inicial'),
(7, 3, 'Generación de compromiso de pago o re-agendamiento registrado');

COMMIT;
