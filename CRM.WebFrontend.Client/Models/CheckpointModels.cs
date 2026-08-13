using System;
using System.Collections.Generic;

namespace CRM.WebFrontend.Client.Models;

public class CheckpointCatalogItem
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Aplica { get; set; } = "Toda campaña nueva";
    public string Etapa { get; set; } = "Activación";
    public string Origen { get; set; } = "INTERNO"; // INTERNO | PROVEEDOR
    public string Alcance { get; set; } = "Venta"; // Venta | Ítem
    public List<string> Bloquea { get; set; } = new(); // Comisión, Liquidación, Alta del servicio
    public bool BloqueaAvance { get; set; }
    public string? RetrocedeA { get; set; }
    public int? DisparaSiKoDe { get; set; }
    public bool Recurrente { get; set; }
    public int? FrecuenciaDias { get; set; }
    public int? MaxOcurrencias { get; set; }
    public string Dueno { get; set; } = "Finanzas";
    public List<string> Pasos { get; set; } = new();
    public string Estado { get; set; } = "ACTIVO"; // ACTIVO | PROPUESTO
}

public class CheckpointInstanceItem
{
    public int CatalogId { get; set; }
    public string Estado { get; set; } = "PENDIENTE"; // PROGRAMADO | PENDIENTE | SUBSANADO | KO
    public int? FechaApertura { get; set; }
    public int? FechaProgramada { get; set; }
    public bool Retro { get; set; }
    public List<bool> PasosCompletados { get; set; } = new();
    public int NumeroOcurrencia { get; set; } = 1;
    public string? DisparadoPor { get; set; }
}

public class SaleItemUiModel
{
    public string Nombre { get; set; } = string.Empty;
    public bool Ancla { get; set; }
    public bool Nucleo { get; set; }
    public string Estado { get; set; } = "PENDIENTE"; // PENDIENTE | ACTIVO | DESCONECTADO
    public int? FechaActivacion { get; set; }
    public int? FechaDesconexion { get; set; }
    public int? FechaRecuperacion { get; set; }
    public decimal ValorLiquidacion { get; set; }
    public int? FechaLiquidado { get; set; }
    public int? FechaDescuentoProveedor { get; set; }
    public decimal ComisionComercial { get; set; }
    public int? FechaPagoComercial { get; set; }
    public int? FechaDescuentoComercial { get; set; }
}

public class SaleHistoryEventUiModel
{
    public int Dia { get; set; }
    public string Detalle { get; set; } = string.Empty;
}

public class SaleUiModel
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Campana { get; set; } = string.Empty;
    public int StageIdx { get; set; }
    public int DayCounter { get; set; } = 1;
    public Dictionary<string, int> FechasPorEtapa { get; set; } = new();
    public List<SaleItemUiModel> Items { get; set; } = new();
    public List<CheckpointInstanceItem> Checkpoints { get; set; } = new();
    public List<SaleHistoryEventUiModel> Historial { get; set; } = new();
}
