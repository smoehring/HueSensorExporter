using System.Diagnostics.Metrics;

namespace HueSensorExporter.service.Models
{
    public class Gauge<T> where T:struct
    {
        private readonly Meter _meter;
        private readonly string _name;
        private readonly string? _unit;
        private readonly string? _description;
        private readonly Dictionary<int, GaugeFunction> _gaugeDirectory;

        public Gauge(Meter meter, string name, string? unit = null, string? description = null)
        {
            _meter = meter;
            _name = name;
            _unit = unit;
            _description = description;
            _gaugeDirectory = new Dictionary<int, GaugeFunction>();
        }

        public void Set(T value, params KeyValuePair<string, object?>[]? tags)
        {
            var hash = tags.GetHashCode();
            if(_gaugeDirectory.ContainsKey(hash))
            {
                _gaugeDirectory[hash].Value = value;
                return;
            }

            _gaugeDirectory.Add(hash, new GaugeFunction(value, tags));
            
            var func = new Func<Measurement<T>>(new Func<Measurement<T>>(() =>
            {
                var id = hash;
                var t = tags;
                return !_gaugeDirectory.ContainsKey(id)
                    ? new Measurement<T>(default, t)
                    : new Measurement<T>(_gaugeDirectory[id].Value, t);
            }));
            _gaugeDirectory[hash].Func = func;
            _gaugeDirectory[hash].Gauge = _meter.CreateObservableGauge(_name, func, _unit, _description);

        }
        

        private class GaugeFunction
        {
            public GaugeFunction(T value, IEnumerable<KeyValuePair<string, object>> tags)
            {
                Tags = tags;
                Value = value;
            }

            private IEnumerable<KeyValuePair<string, object>> Tags { get; set; }
            public ObservableGauge<T>? Gauge { get; set; }
            public Func<Measurement<T>>? Func { get; set; }
            public T Value { get; set; }
        }
    }
}
