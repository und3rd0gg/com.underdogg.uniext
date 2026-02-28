using System;
using System.Collections.Generic;
using System.Reflection;
using com.underdogg.uniext.Runtime.Dictionary;
using com.underdogg.uniext.Runtime.Extensions;
using NUnit.Framework;

namespace com.underdogg.uniext.Tests.Runtime
{
    public sealed class UniExtRuntimeTests
    {
        [Test]
        public void UniDict_RebuildFromEntries_WithDuplicateKeepLast_UsesLastValue()
        {
            var dict = new UniDict<string, int> {
                DeserializationDuplicateKeyPolicy = UniDict<string, int>.DuplicateKeyPolicy.KeepLast,
                LogValidationWarnings = false
            };

            SetEntries(dict, new List<DictEntry<string, int>> {
                new() { Key = "A", Value = 10 },
                new() { Key = "A", Value = 20 }
            });

            dict.RebuildFromEntries();

            Assert.That(dict.Count, Is.EqualTo(1));
            Assert.That(dict["A"], Is.EqualTo(20));
        }

        [Test]
        public void UniDict_CopyTo_ThrowsWhenArrayIsInvalid()
        {
            var dict = new UniDict<string, int>();
            dict.Add("A", 1);
            dict.Add("B", 2);

            Assert.Throws<ArgumentNullException>(() => dict.CopyTo(null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => dict.CopyTo(new KeyValuePair<string, int>[2], -1));
            Assert.Throws<ArgumentException>(() => dict.CopyTo(new KeyValuePair<string, int>[1], 0));
        }

        [Test]
        public void ObservableVariable_SetValue_DoesNotNotifyWhenValueIsUnchanged()
        {
            var variable = new ObservableVariable<int>(5);
            var notifications = 0;
            variable.OnChanged += _ => notifications++;

            var changed = variable.SetValue(5);

            Assert.That(changed, Is.False);
            Assert.That(notifications, Is.EqualTo(0));
        }

        [Test]
        public void ObservableVariable_SetValue_ForceNotify_NotifiesWhenValueIsUnchanged()
        {
            var variable = new ObservableVariable<int>(5);
            var notifications = 0;
            variable.OnChanged += _ => notifications++;

            var changed = variable.SetValue(5, forceNotify: true);

            Assert.That(changed, Is.False);
            Assert.That(notifications, Is.EqualTo(1));
        }

        [Test]
        public void ListExt_PopLast_RemovesLastItem()
        {
            var values = new List<int> { 1, 2, 3 };

            var last = values.PopLast();

            Assert.That(last, Is.EqualTo(3));
            Assert.That(values, Is.EqualTo(new[] { 1, 2 }));
        }

        private static void SetEntries<TKey, TValue>(UniDict<TKey, TValue> dict, List<DictEntry<TKey, TValue>> entries)
            where TKey : notnull
        {
            var field = typeof(UniDict<TKey, TValue>).GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                Assert.Fail("Failed to locate UniDict internal '_entries' field.");

            field.SetValue(dict, entries);
        }
    }
}
