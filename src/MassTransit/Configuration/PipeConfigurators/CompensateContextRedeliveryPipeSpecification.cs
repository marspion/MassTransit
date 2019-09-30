namespace MassTransit.PipeConfigurators
{
    using System.Collections.Generic;
    using System.Threading;
    using Courier;
    using Courier.Contexts;
    using Courier.Contracts;
    using GreenPipes;
    using GreenPipes.Configurators;
    using GreenPipes.Observers;
    using GreenPipes.Specifications;
    using Pipeline.Filters;


    public class CompensateContextRedeliveryPipeSpecification<TLog> :
        ExceptionSpecification,
        IRetryConfigurator,
        IPipeSpecification<CompensateContext<TLog>>
        where TLog : class
    {
        readonly RetryObservable _observers;
        RetryPolicyFactory _policyFactory;

        public CompensateContextRedeliveryPipeSpecification()
        {
            _observers = new RetryObservable();
        }

        public void Apply(IPipeBuilder<CompensateContext<TLog>> builder)
        {
            var retryPolicy = _policyFactory(Filter);

            var policy = new ConsumeContextRetryPolicy<CompensateContext<TLog>, RetryCompensateContext<TLog>>(retryPolicy, CancellationToken.None, Factory);

            builder.AddFilter(new RedeliveryRetryFilter<CompensateContext<TLog>, RoutingSlip>(policy, _observers));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_policyFactory == null)
                yield return this.Failure("RetryPolicy", "must not be null");
        }

        public void SetRetryPolicy(RetryPolicyFactory factory)
        {
            _policyFactory = factory;
        }

        ConnectHandle IRetryObserverConnector.ConnectRetryObserver(IRetryObserver observer)
        {
            return _observers.Connect(observer);
        }

        static RetryCompensateContext<TLog> Factory(CompensateContext<TLog> context, IRetryPolicy retryPolicy, RetryContext retryContext)
        {
            return context as RetryCompensateContext<TLog> ?? new RetryCompensateContext<TLog>(context, retryPolicy, retryContext);
        }
    }
}
