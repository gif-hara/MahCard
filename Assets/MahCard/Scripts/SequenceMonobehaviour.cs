using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnitySequencerSystem;

namespace MahCard
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SequenceMonobehaviour : MonoBehaviour
    {
        [SerializeReference, SubclassSelector]
        private List<ISequence> sequences;

        public UniTask PlayAsync(CancellationToken scope = default)
        {
            return PlayAsync(new Container(), scope);
        }

        public UniTask PlayAsync(Container container, CancellationToken scope = default)
        {
            var sequencer = new Sequencer(container, sequences);
            return sequencer.PlayAsync(scope);
        }
    }
}
