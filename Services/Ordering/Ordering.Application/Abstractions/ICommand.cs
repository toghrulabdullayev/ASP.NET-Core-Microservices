namespace Ordering.Application.Abstractions
{
	// a marker interface for commands not returning results
	public interface ICommand { }

	// a marker interface for commands returning results
	public interface ICommand<TResult> { }
}
