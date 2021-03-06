﻿using System.Collections;
using System.Collections.Generic;

namespace Mikodev.Network
{
    internal sealed class DictionaryAdapter<TK, TV> : IEnumerable<KeyValuePair<byte[], object>>
    {
        private readonly IPacketConverter converter;
        private readonly IEnumerable<KeyValuePair<TK, TV>> dictionary;

        internal DictionaryAdapter(IPacketConverter converter, IEnumerable<KeyValuePair<TK, TV>> dictionary)
        {
            this.converter = converter;
            this.dictionary = dictionary;
        }

        private IEnumerator<KeyValuePair<byte[], object>> Enumerator()
        {
            if (converter is IPacketConverter<TK> gen)
                foreach (var i in dictionary)
                    yield return new KeyValuePair<byte[], object>(gen.GetBytesWrap(i.Key), i.Value);
            else
                foreach (var i in dictionary)
                    yield return new KeyValuePair<byte[], object>(converter.GetBytesWrap(i.Key), i.Value);
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => Enumerator();

        IEnumerator<KeyValuePair<byte[], object>> IEnumerable<KeyValuePair<byte[], object>>.GetEnumerator() => Enumerator();
    }
}
