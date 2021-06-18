using AutoMapper;
using HtmlRaportGenerator.Tools.Mapper;
using System.Collections.Generic;

namespace HtmlRaportGenerator.Tools.StateHelper.Implementations
{
    //todo testy tooli
    public class PreviousStateCollectionService<T> : IPreviousState<ICollection<T>>
        where T : class, IMapFrom<T>
    {
        private readonly IMapper _mapper;

        private int _originalHashCode;

        private bool _isValueChanged;

        public ICollection<T> OriginalValue { get; private set; } = null!;

        public ICollection<T> CurrentValue { get; private set; } = null!;

        public PreviousStateCollectionService(IMapper mapper)
             => _mapper = mapper;

        public bool IsValueChanged() => _isValueChanged;

        public void Load(ICollection<T> current)
        {
            current.CheckNotNull(nameof(current));

            OriginalValue = _mapper.Map<ICollection<T>>(current);
            CurrentValue = current;

            _originalHashCode = OriginalValue.GetCollectionHashCode();

            UpdateIsValueChangedInternal();
        }

        public void Rollback()
        {
            _mapper.Map(OriginalValue, CurrentValue);

            _isValueChanged = false;
        }

        public void Update()
        {
            _mapper.Map(CurrentValue, OriginalValue);

            _originalHashCode = OriginalValue.GetCollectionHashCode();

            UpdateIsValueChangedInternal();
        }

        public void UpdateIsValueChanged()
        {
            if (OriginalValue is null || CurrentValue is null)
            {
                _isValueChanged = false;
            }
            else
            {
                UpdateIsValueChangedInternal();
            }
        }

        private void UpdateIsValueChangedInternal()
            => _isValueChanged =
            OriginalValue.Count != CurrentValue.Count ||
            _originalHashCode != CurrentValue.GetCollectionHashCode();
    }
}
