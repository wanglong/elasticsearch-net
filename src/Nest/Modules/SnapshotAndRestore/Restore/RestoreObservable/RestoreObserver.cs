using System;

namespace Nest
{
	public class RestoreObserver : CoordinatedRequestObserverBase<IRecoveryStatusResponse>
	{
		public RestoreObserver(
			Action<IRecoveryStatusResponse> onNext = null,
			Action<Exception> onError = null,
			System.Action completed = null)
			: base(onNext, onError, completed)
		{

		}
	}
}
