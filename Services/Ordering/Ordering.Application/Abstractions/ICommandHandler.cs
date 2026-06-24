namespace Ordering.Application.Abstractions
{
	// "in TCommand" marks TCommand as contravariant, meaning it can only be used as an
	// input/parameter type, not as a return type

	// "where TCommand : ICommand" constrains TCommand so that only types implementing
	// ICommand can be used
	public interface ICommandHandler<in TCommand>
		where TCommand : ICommand
	{
		Task Handle(TCommand command, CancellationToken cancellationToken);
	}
}
