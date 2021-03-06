﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Mikodev.Network
{
    internal class Enumerable : IEnumerable
    {
        protected readonly PacketReader reader;
        protected readonly IPacketConverter converter;

        internal Enumerable(PacketReader reader, IPacketConverter converter)
        {
            this.reader = reader;
            this.converter = converter;
        }

        private static IEnumerator Enumerator(byte[] buffer, int offset, int count, int define, IPacketConverter converter)
        {
            for (int idx = 0; idx < count; idx++)
                yield return converter.GetValueWrap(buffer, offset + define * idx, define);
        }

        private static IEnumerator Enumerator(List<PacketReader> list, IPacketConverter converter)
        {
            for (int idx = 0; idx < list.Count; idx++)
                yield return converter.GetValueWrap(list[idx].element);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var def = converter.Length;
            if (def < 1)
                return Enumerator(reader.GetList(), converter);
            var ele = reader.element;
            var sum = Math.DivRem(ele.length, def, out var rem);
            if (rem != 0)
                throw PacketException.Overflow();
            return Enumerator(ele.buffer, ele.offset, sum, def, converter);
        }
    }

    internal sealed class Enumerable<T> : Enumerable, IEnumerable<T>
    {
        internal Enumerable(PacketReader reader, IPacketConverter converter) : base(reader, converter) { }

        private static IEnumerator<T> Enumerator(byte[] buffer, int offset, int count, int define, IPacketConverter converter)
        {
            if (converter is IPacketConverter<T> gen)
                for (int idx = 0; idx < count; idx++)
                    yield return gen.GetValueWrap(buffer, offset + define * idx, define);
            else
                for (int idx = 0; idx < count; idx++)
                    yield return (T)converter.GetValueWrap(buffer, offset + define * idx, define);
        }

        private static IEnumerator<T> Enumerator(List<PacketReader> list, IPacketConverter converter)
        {
            if (converter is IPacketConverter<T> gen)
                for (int idx = 0; idx < list.Count; idx++)
                    yield return gen.GetValueWrap(list[idx].element);
            else
                for (int idx = 0; idx < list.Count; idx++)
                    yield return (T)converter.GetValueWrap(list[idx].element);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var def = converter.Length;
            if (def < 1)
                return Enumerator(reader.GetList(), converter);
            var ele = reader.element;
            var sum = Math.DivRem(ele.length, def, out var rem);
            if (rem != 0)
                throw PacketException.Overflow();
            else return Enumerator(ele.buffer, ele.offset, sum, def, converter);
        }
    }
}
