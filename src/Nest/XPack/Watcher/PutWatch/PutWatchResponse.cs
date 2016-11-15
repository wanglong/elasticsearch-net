﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<PutWatchResponse>))]
	public interface IPutWatchResponse : IResponse
	{
		[JsonProperty("_id")]
		Id Id { get; }

		[JsonProperty("_version")]
		int Version { get; }

		[JsonProperty("created")]
		bool Created { get; }
	}

	public class PutWatchResponse : ResponseBase, IPutWatchResponse
	{
		public Id Id { get; internal set; }

		public int Version { get; internal set; }

		public bool Created { get; internal set; }
	}
}
