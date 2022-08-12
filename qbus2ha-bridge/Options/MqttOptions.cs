using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qbus2ha.Options
{
    public record MqttOptions(string Host, int Port, string Username, string Password);
}
