using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Elasticsearch.Net;

namespace Nest
{
	[JsonObject]
	[JsonConverter(typeof(ReadAsTypeJsonConverter<SearchTransform>))]
	public interface ISearchTransform : ITransform
	{
		[JsonProperty("search_type")]
		SearchType? SearchType { get; set; }

		[JsonProperty("indices")]
		IEnumerable<IndexName> Indices { get; set; }

		[JsonProperty("indices_options")]
		IIndicesOptions IndicesOptions { get; set; }

		[JsonProperty("type")]
		IEnumerable<TypeName> Type { get; set; }

		[JsonProperty("body")]
		ISearchRequest Body { get; set; }

		[JsonProperty("template")]
		ISearchTemplateRequest Template { get; set; }

		[JsonProperty("timeout")]
		Time Timeout { get; set; }
	}

	public class SearchTransform : TransformBase, ISearchTransform
	{
		public SearchType? SearchType { get; set; }
		public IEnumerable<IndexName> Indices { get; set; }
		public IIndicesOptions IndicesOptions { get; set; }
		public IEnumerable<TypeName> Type { get; set; }
		public ISearchRequest Body { get; set; }
		public ISearchTemplateRequest Template { get; set; }
		public Time Timeout { get; set; }

		internal override void WrapInContainer(ITransformContainer container) => container.Search = this;
	}

	public class SearchTransformDescriptor : DescriptorBase<SearchTransformDescriptor, ISearchTransform>, ISearchTransform
	{
		SearchType? ISearchTransform.SearchType { get; set; }
		IEnumerable<IndexName> ISearchTransform.Indices { get; set; }
		IIndicesOptions ISearchTransform.IndicesOptions { get; set; }
		IEnumerable<TypeName> ISearchTransform.Type { get; set; }
		ISearchRequest ISearchTransform.Body { get; set; }
		ISearchTemplateRequest ISearchTransform.Template { get; set; }
		Time ISearchTransform.Timeout { get; set; }

		public SearchTransformDescriptor SearchType(SearchType searchType) => Assign(a => a.SearchType = searchType);

		public SearchTransformDescriptor Indices(IEnumerable<IndexName> indices) => Assign(a => a.Indices = indices);

		public SearchTransformDescriptor Indices(params IndexName[] indices) => Assign(a => a.Indices = indices);

		public SearchTransformDescriptor Indices<T>() => Assign(a => a.Indices = new IndexName[] { typeof(T) });

		public SearchTransformDescriptor IndicesOptions(Func<IndicesOptionsDescriptor, IIndicesOptions> selector) =>
			Assign(a => a.IndicesOptions = selector.InvokeOrDefault(new IndicesOptionsDescriptor()));

		public SearchTransformDescriptor Type(IEnumerable<TypeName> type) => Assign(a => a.Type = type);

		public SearchTransformDescriptor Type(params TypeName[] type) => Assign(a => a.Type = type);

		public SearchTransformDescriptor Type<T>() => Assign(a => a.Type = new TypeName[] { typeof(T) });

		public SearchTransformDescriptor Body<T>(Func<SearchDescriptor<T>, ISearchRequest> selector) where T : class =>
			Assign(a => a.Body = selector.InvokeOrDefault(new SearchDescriptor<T>()));

		public SearchTransformDescriptor Template<T>(Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest> selector) where T : class =>
			Assign(a => a.Template = selector.InvokeOrDefault(new SearchTemplateDescriptor<T>()));

		public SearchTransformDescriptor Timeout(Time timeout) =>
			Assign(a => a.Timeout = timeout);
	}
}
