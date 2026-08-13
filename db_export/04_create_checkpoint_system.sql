-- ============================================================================
-- NYX CRM - CHECKPOINT SYSTEM SCHEMAS, TABLES, AND SEED DATA
-- ============================================================================

BEGIN;

-- 1. Catálogo de checkpoints (Reglas Maestras)
CREATE TABLE IF NOT EXISTS sales_service.checkpoint_catalog (
    id_checkpoint_catalog SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    applies_to VARCHAR(100) NOT NULL,
    trigger_stage_id INTEGER NOT NULL REFERENCES sales_service.order_status(id_status) ON DELETE RESTRICT,
    origin VARCHAR(50) NOT NULL DEFAULT 'INTERNO', -- INTERNO, PROVEEDOR, etc.
    scope VARCHAR(50) NOT NULL DEFAULT 'Venta',
    blocks_progress BOOLEAN NOT NULL DEFAULT FALSE, -- bloquea avance de etapa
    blocks_commission BOOLEAN NOT NULL DEFAULT FALSE, -- bloquea comisiones
    blocks_activation BOOLEAN NOT NULL DEFAULT FALSE, -- bloquea alta de servicio
    blocks_liquidation BOOLEAN NOT NULL DEFAULT FALSE, -- bloquea liquidaciones
    rollback_stage_id INTEGER REFERENCES sales_service.order_status(id_status) ON DELETE SET NULL, -- retrocede a esta etapa al rechazar
    trigger_on_ko_catalog_id INTEGER REFERENCES sales_service.checkpoint_catalog(id_checkpoint_catalog) ON DELETE SET NULL, -- se dispara si este checkpoint sale KO
    owner_department VARCHAR(100) NOT NULL,
    required_steps_count INTEGER NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE sales_service.checkpoint_catalog OWNER TO ronald;

-- 2. Checklists o Pasos del checkpoint
CREATE TABLE IF NOT EXISTS sales_service.checkpoint_step (
    id_checkpoint_step SERIAL PRIMARY KEY,
    id_checkpoint_catalog INTEGER NOT NULL REFERENCES sales_service.checkpoint_catalog(id_checkpoint_catalog) ON DELETE CASCADE,
    step_index INTEGER NOT NULL,
    description VARCHAR(255) NOT NULL,
    UNIQUE (id_checkpoint_catalog, step_index)
);

ALTER TABLE sales_service.checkpoint_step OWNER TO ronald;

-- 3. Instancias de Checkpoint por Orden
CREATE TABLE IF NOT EXISTS sales_service.order_checkpoint (
    id_order_checkpoint SERIAL PRIMARY KEY,
    id_order INTEGER NOT NULL REFERENCES sales_service.sales_order(id_order) ON DELETE CASCADE,
    id_checkpoint_catalog INTEGER NOT NULL REFERENCES sales_service.checkpoint_catalog(id_checkpoint_catalog) ON DELETE RESTRICT,
    status VARCHAR(50) NOT NULL DEFAULT 'PENDIENTE', -- PENDIENTE, EN_PROCESO, APROBADO, RECHAZADO, BLOQUEADO
    comments TEXT,
    checked_by INTEGER REFERENCES user_service.users(id_user) ON DELETE SET NULL,
    checked_at TIMESTAMP WITH TIME ZONE,
    steps_completed INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE sales_service.order_checkpoint OWNER TO ronald;
CREATE INDEX IF NOT EXISTS idx_order_checkpoint_order ON sales_service.order_checkpoint(id_order);
CREATE INDEX IF NOT EXISTS idx_order_checkpoint_status ON sales_service.order_checkpoint(status);

-- 4. Estado de pasos individuales de un checkpoint
CREATE TABLE IF NOT EXISTS sales_service.order_checkpoint_step_status (
    id_order_checkpoint INTEGER NOT NULL REFERENCES sales_service.order_checkpoint(id_order_checkpoint) ON DELETE CASCADE,
    step_index INTEGER NOT NULL,
    is_completed BOOLEAN NOT NULL DEFAULT FALSE,
    updated_by INTEGER REFERENCES user_service.users(id_user) ON DELETE SET NULL,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_order_checkpoint, step_index)
);

ALTER TABLE sales_service.order_checkpoint_step_status OWNER TO ronald;

-- ============================================================================
-- SEEDING CATALOG DATA
-- ============================================================================

-- Limpiar catálogo si es necesario en pruebas
TRUNCATE sales_service.checkpoint_step CASCADE;
TRUNCATE sales_service.checkpoint_catalog CASCADE;

-- 1. Auditoría de audio
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (1, 'Auditoría de audio', 'Alarmas', 9, 'INTERNO', 'Venta', FALSE, TRUE, TRUE, FALSE, NULL, NULL, 'Finanzas', 5);

INSERT INTO sales_service.checkpoint_step (id_checkpoint_catalog, step_index, description) VALUES
(1, 1, 'Verificación de la nitidez de la voz del titular'),
(1, 2, 'Confirmación del consentimiento explícito de contratación'),
(1, 3, 'Validación de los datos bancarios dictados'),
(1, 4, 'Verificación de lectura obligatoria del RGPD y condiciones de uso'),
(1, 5, 'Confirmación de coincidencia de DNI/NIE expresado verbalmente');

-- 2. Revisión de supervisor
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (2, 'Revisión de supervisor', 'Toda campaña nueva', 2, 'INTERNO', 'Venta', TRUE, FALSE, FALSE, FALSE, NULL, NULL, 'Operaciones', 0);

-- 3. Confirmación de firma de contrato
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (3, 'Confirmación de firma de contrato', 'Toda campaña nueva', 8, 'PROVEEDOR', 'Venta', TRUE, TRUE, FALSE, TRUE, NULL, NULL, 'Backoffice', 0);

-- 4. Seguimiento de satisfacción postventa
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (4, 'Seguimiento de satisfacción postventa', 'Toda campaña nueva', 9, 'INTERNO', 'Venta', FALSE, FALSE, FALSE, FALSE, NULL, NULL, 'Postventa', 3);

INSERT INTO sales_service.checkpoint_step (id_checkpoint_catalog, step_index, description) VALUES
(4, 1, 'Llamada de bienvenida realizada con éxito'),
(4, 2, 'Confirmación de instalación correcta de equipos/servicios'),
(4, 3, 'Validación del índice NPS inicial favorable');

-- 5. Validación de Backoffice
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (5, 'Validación de Backoffice', 'Toda campaña nueva', 3, 'INTERNO', 'Venta', TRUE, FALSE, FALSE, FALSE, 2, NULL, 'Backoffice', 0);

-- 6. Validación de documentos externos
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (6, 'Validación de documentos externos', 'Toda campaña nueva', 4, 'INTERNO', 'Venta', TRUE, FALSE, FALSE, FALSE, 5, NULL, 'Backoffice', 0);

-- 7. Gestión de recuperación
INSERT INTO sales_service.checkpoint_catalog (id_checkpoint_catalog, name, applies_to, trigger_stage_id, origin, scope, blocks_progress, blocks_commission, blocks_activation, blocks_liquidation, rollback_stage_id, trigger_on_ko_catalog_id, owner_department, required_steps_count)
VALUES (7, 'Gestión de recuperación', 'Toda campaña nueva', 9, 'INTERNO', 'Venta', FALSE, TRUE, FALSE, FALSE, NULL, 4, 'Postventa', 3);

INSERT INTO sales_service.checkpoint_step (id_checkpoint_catalog, step_index, description) VALUES
(7, 1, 'Intento de contacto telefónico registrado'),
(7, 2, 'Verificación de motivo de impago o incidencia técnica inicial'),
(7, 3, 'Generación de compromiso de pago o re-agendamiento registrado');

COMMIT;
