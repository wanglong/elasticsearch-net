using System;

namespace Nest
{
	public class SnapshotObserver : CoordinatedRequestObserverBase<ISnapshotStatusResponse>
	{
		public SnapshotObserver(
			Action<ISnapshotStatusResponse> onNext = null,
			Action<Exception> onError = null,
			System.Action completed = null)
			: base(onNext, onError, completed)
		{

		}
	}
}
