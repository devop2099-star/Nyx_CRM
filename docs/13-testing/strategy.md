# Estrategia de Pruebas y Cobertura (Testing)

## 1. Pirámide de Pruebas

```text
       /\
      /  \     Pruebas E2E (Playwright / Selenium)
     /    \    Flujo Asesor -> Supervisor -> Backoffice
    /------\
   /        \   Pruebas de Integración (WebApplicationFactory + Testcontainers PG)
  /----------\  Controllers, Handlers y EF Core Repositories
 /------------\
/--------------\ Pruebas Unitarias (xUnit + Moq)
                 Domain Entities, Value Objects, Business Rules
```

---

## 2. Cobertura Mínima Exigida
- **Capa Domain & Application**: 85% de cobertura unitaria.
- **Capa Api Controllers**: 75% de cobertura de integración.

---

## 3. Informes de Ejecución E2E Realizados
- **Flujo Asesor y Formularios**: Pruebas validadas con 100% de éxito en carga de pre-ventas y evidencias.
- **Flujo Supervisor & BAC (Backoffice Activations)**: Pruebas de auditoría de audio y aprobación de órdenes validadas exitosamente.
