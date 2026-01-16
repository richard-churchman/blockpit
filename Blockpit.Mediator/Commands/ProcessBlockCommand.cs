namespace Blockpit.Mediator.Commands
{

    namespace Blockpit.Mediator.Commands
    {
        using MediatR;
        using Models;
        public record ProcessBlockCommand(BlockTick BlockTick) : IRequest<Unit>;
    }
}
