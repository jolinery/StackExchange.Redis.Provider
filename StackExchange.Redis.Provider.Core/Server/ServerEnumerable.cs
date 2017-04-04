
using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis.Provider.Core.Configuration;
using StackExchange.Redis;
namespace StackExchange.Redis.Provider.Core.Server
{
    public class ServerEnumerable : IEnumerable<IServer>
    {
		private readonly ConnectionMultiplexer _multiplexer;
		private readonly ServerEnumerationStrategy.TargetRoleOptions _targetRole;
		private readonly ServerEnumerationStrategy.UnreachableServerActionOptions _unreachableServerAction;

		public ServerEnumerable(
			ConnectionMultiplexer multiplexer,
			ServerEnumerationStrategy.TargetRoleOptions targetRole,
			ServerEnumerationStrategy.UnreachableServerActionOptions unreachableServerAction)
		{
			this._multiplexer = multiplexer;
			this._targetRole = targetRole;
			this._unreachableServerAction = unreachableServerAction;
		}

		public IEnumerator<IServer> GetEnumerator()
		{
			foreach (var endPoint in _multiplexer.GetEndPoints())
			{
				var server = _multiplexer.GetServer(endPoint);
				if (_targetRole == ServerEnumerationStrategy.TargetRoleOptions.PreferSlave)
				{
					if (!server.IsSlave)
						continue;
				}
				if (_unreachableServerAction == ServerEnumerationStrategy.UnreachableServerActionOptions.IgnoreIfOtherAvailable)
				{
					if (!server.IsConnected || !server.Features.Scan)
						continue;
				}

				yield return server;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

    }
}
