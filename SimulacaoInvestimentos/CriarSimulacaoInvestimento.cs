using Carter;
using FluentValidation;
using Mapster;

namespace simulador_financeiro.SimulacaoInvestimentos;

public static class CriarSimulacaoInvestimento
{
    public record Request(
        string Descricao,
        decimal ValorAplicado,
        decimal ValorAporteMensal,
        int PeriodoEmMeses,
        decimal TaxaAnual,
        decimal TaxaCdi,
        decimal TaxaInvestimento,
        bool TaxaIrFixa = false);

    private class Response
    {
        private decimal _valorAplicado;
        private decimal _valorAporteMensal;
        private int _periodoEmMeses;
        private decimal _valorRendimentoLiquido;
        private decimal _valorTotalBruto;
        private decimal _valorTotalLiquido;
        private decimal _taxaAnual;
        private decimal _taxaMensal;
        private decimal _taxaAnualReal;
        private decimal _taxaMensalReal;
        private decimal _taxaIr;

        public Guid Id { get; set; }
        public string? Descricao { get; set; }
        
        public decimal ValorAplicado
        {
            get => _valorAplicado.Round2();
            set => _valorAplicado = value;
        }

        public decimal ValorAporteMensal
        {
            get => _valorAporteMensal.Round2();
            set => _valorAporteMensal = value;
        }

        public int PeriodoEmMeses
        {
            get => _periodoEmMeses;
            set => _periodoEmMeses = value;
        }

        public decimal ValorRendimentoLiquido
        {
            get => _valorRendimentoLiquido.Round2();
            set => _valorRendimentoLiquido = value;
        }

        public decimal ValorTotalBruto
        {
            get => _valorTotalBruto.Round2();
            set => _valorTotalBruto = value;
        }
        public decimal ValorTotalLiquido
        {
            get => _valorTotalLiquido.Round2();
            set => _valorTotalLiquido = value;
        }
        public decimal TaxaAnual
        {
            get => _taxaAnual.Round2();
            set => _taxaAnual = value;
        }

        public decimal TaxaMensal
        {
            get => _taxaMensal.Round2();
            set => _taxaMensal = value;
        }
        public decimal TaxaAnualReal
        {
            get => _taxaAnualReal.Round2();
            set => _taxaAnualReal = value;
        }

        public decimal TaxaMensalReal
        {
            get => _taxaMensalReal.Round2();
            set => _taxaMensalReal = value;
        }
        public decimal TaxaIr
        {
            get => _taxaIr.Round2();
            set => _taxaIr = value;
        }
    }
    
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Descricao).NotEmpty();
            RuleFor(x => x.ValorAplicado).GreaterThan(0);
            RuleFor(x => x.ValorAporteMensal).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TaxaAnual).GreaterThan(0).When(x => x.TaxaCdi <= 0)
                .WithMessage("Taxa anual deve ser maior que 0 quando a taxa do CDI é 0");
            RuleFor(x => x.TaxaCdi).GreaterThan(0).When(x => x.TaxaAnual <= 0)
                .WithMessage("Taxa CDI deve ser maior que 0 quando a taxa anual é 0");
            RuleFor(x => x.TaxaInvestimento).GreaterThan(0).When(x => x.TaxaCdi > 0 && x.TaxaAnual <= 0)
                .WithMessage("Taxa do Investimento deve ser maior que 0 quando a taxa do CDI é informada");
        }
    }
    
    public sealed class Endpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("simulacao-investimentos", Handler).WithTags("Simulação Investimentos");
        }
    }
    
    private static IResult Handler(Request request, IValidator<Request> validator)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

        var simulacaoInvestimento = request.TaxaAnual > 0 ?
            SimulacaoInvestimento.ComSuaTaxa(request.Descricao, request.ValorAplicado, request.ValorAporteMensal,
                request.PeriodoEmMeses, request.TaxaAnual, request.TaxaIrFixa) :
            SimulacaoInvestimento.ComBaseNoCdi(request.Descricao, request.ValorAplicado, request.ValorAporteMensal,
                request.PeriodoEmMeses, request.TaxaCdi, request.TaxaInvestimento, request.TaxaIrFixa);

        return Results.Ok(simulacaoInvestimento.Adapt<Response>());
    }
}