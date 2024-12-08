namespace simulador_financeiro.SimulacaoInvestimentos;

public class SimulacaoInvestimento
{
    #region PROPERTIES
    public Guid Id { get; set; }
    public string? Descricao { get; set; }
    public decimal ValorAplicado { get; set; }
    public decimal ValorAporteMensal { get; set; }
    public decimal ValorTotalAplicado { get; set; }
    public decimal TaxaAnual { get; set; }
    public decimal TaxaMensal { get; set; }
    public decimal TaxaMensalCapitalizacao { get; set; }
    public decimal TaxaAnualReal { get; set; }
    public decimal TaxaMensalReal { get; set; }
    public int PeriodoEmMeses { get; set; }
    public decimal TaxaIr { get; set; }
    public decimal ValorTotalSemAporteBruto { get; set; }
    public decimal ValorTotalAporteBruto { get; set; }
    public decimal ValorTotalBruto { get; set; }
    public decimal ValorTotalLiquido { get; set; }
    public decimal ValorRendimentoBruto { get; set; }
    public decimal ValorRendimentoLiquido { get; set; }
    public decimal ValorIrCobrado { get; set; }
    #endregion
    
    private void CalcularValoresInvestimento(bool taxaIrFixa)
    {
        if (taxaIrFixa) TaxaIr = 22.5M; 
        else CalcularIr();

        TaxaMensal = TaxaAnual/12M;
        var taxaMensalCalculo = (decimal)Math.Pow((double)(1 + TaxaAnual / 100), 1.0 / 12.0) - 1;
        TaxaMensalCapitalizacao = taxaMensalCalculo * 100;
        
        ValorTotalSemAporteBruto = ValorAplicado * (decimal)Math.Pow((double)(1 + taxaMensalCalculo), PeriodoEmMeses);
        ValorTotalAporteBruto = ValorAporteMensal * (decimal)((Math.Pow((double)(1 + taxaMensalCalculo), PeriodoEmMeses) - 1) / (double)taxaMensalCalculo);
        ValorTotalBruto = ValorTotalSemAporteBruto + ValorTotalAporteBruto;
        ValorRendimentoBruto = ValorTotalBruto - ValorAplicado;
        
        ValorIrCobrado = TaxaIr / 100M * ValorRendimentoBruto;
        
        ValorRendimentoLiquido = ValorRendimentoBruto - ValorIrCobrado;
        ValorTotalLiquido = ValorAplicado + ValorRendimentoLiquido;
        
        ValorTotalAplicado = ValorAplicado + ValorAporteMensal * PeriodoEmMeses;
        
        TaxaAnualReal = ((decimal)Math.Pow((double)ValorTotalLiquido / (double)ValorTotalAplicado, 1.0/(PeriodoEmMeses/12.0))-1) * 100;
        TaxaMensalReal = TaxaAnualReal / 12;
    }
    
    private void CalcularIr()
    {
        var periodoEmDias = PeriodoEmMeses * 30;
        TaxaIr = periodoEmDias switch
        {
            > 180 and <= 360 => 20M,
            > 360 and <= 720 => 17.5M,
            > 720 => 15M,
            _ => 22.5M
        };
    }
    
    private SimulacaoInvestimento(string descricao, decimal valorAplicado, decimal valorAporteMensal, 
        int periodoEmMeses, decimal taxaAnual, bool taxaIrFixa)
    {
        Id = Guid.NewGuid();
        Descricao = descricao;
        ValorAplicado = valorAplicado;
        ValorAporteMensal = valorAporteMensal;
        PeriodoEmMeses = periodoEmMeses;
        TaxaAnual = taxaAnual;
        CalcularValoresInvestimento(taxaIrFixa);
    }
    
    private SimulacaoInvestimento(string descricao, decimal valorAplicado, decimal valorAporteMensal, 
        int periodoEmMeses, decimal taxaCdi, decimal taxaInvestimento, bool taxaIrFixa)
    {
        Id = Guid.NewGuid();
        Descricao = descricao;
        ValorAplicado = valorAplicado;
        ValorAporteMensal = valorAporteMensal;
        TaxaAnual = taxaCdi * (taxaInvestimento / 100);
        PeriodoEmMeses = periodoEmMeses;
        CalcularValoresInvestimento(taxaIrFixa);
    }

    public static SimulacaoInvestimento ComSuaTaxa(string descricao, decimal valorAplicado, decimal valorAporteMensal,
        int periodoEmMeses, decimal taxaAnual, bool taxaIrFixa = false)
    {
        return new SimulacaoInvestimento(descricao, valorAplicado, valorAporteMensal, periodoEmMeses, taxaAnual, taxaIrFixa);
    }
    
    public static SimulacaoInvestimento ComBaseNoCdi(string descricao, decimal valorAplicado, decimal valorAporteMensal,
        int periodoEmMeses, decimal taxaCdi, decimal taxaInvestimento, bool taxaIrFixa = false)
    {
        return new SimulacaoInvestimento(descricao, valorAplicado, valorAporteMensal, periodoEmMeses, taxaCdi, taxaInvestimento, taxaIrFixa);
    }
}