namespace MassTransit.Courier.Contexts
{
    using System;
    using System.Collections.Generic;
    using GreenPipes.Payloads;


    public class CompensateContextProxy<TLog> :
        CourierContextProxy,
        CompensateContext<TLog>
        where TLog : class
    {
        readonly CompensateContext<TLog> _context;

        protected CompensateContextProxy(CompensateContext<TLog> context)
            : base(context)
        {
            _context = context;
        }

        public CompensateContextProxy(CompensateContext<TLog> context, IPayloadCache payloadCache)
            : base(context, payloadCache)
        {
            _context = context;
        }

        TLog CompensateContext<TLog>.Log => _context.Log;

        CompensateActivityContext<TActivity, TLog> CompensateContext<TLog>.CreateActivityContext<TActivity>(TActivity activity)
        {
            return new HostCompensateActivityContext<TActivity, TLog>(activity, this);
        }

        CompensationResult CompensateContext.Compensated()
        {
            return _context.Compensated();
        }

        CompensationResult CompensateContext.Compensated(object values)
        {
            return _context.Compensated(values);
        }

        CompensationResult CompensateContext.Compensated(IDictionary<string, object> variables)
        {
            return _context.Compensated(variables);
        }

        CompensationResult CompensateContext.Failed()
        {
            return _context.Failed();
        }

        CompensationResult CompensateContext.Failed(Exception exception)
        {
            return _context.Failed(exception);
        }

        CompensationResult CompensateContext.Result
        {
            get => _context.Result;
            set => _context.Result = value;
        }
    }
}
