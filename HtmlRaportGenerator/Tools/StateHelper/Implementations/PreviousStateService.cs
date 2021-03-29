using AutoMapper;
using HtmlRaportGenerator.Tools.Mapper;

namespace HtmlRaportGenerator.Tools.StateHelper.Implementations
{
    ///<inheritdoc cref="IPreviousState{T}"/>
    public class PreviousStateService<T> : IPreviousState<T>
        where T : class, IMapFrom<T>
    {
        private readonly IMapper _mapper;

        private int _originalHashCode;

        public T OriginalValue { get; private set; } = null!;

        public T CurrentValue { get; private set; } = null!;

        public PreviousStateService(IMapper mapper)
            => _mapper = mapper;

        public bool IsValueChanged()
            => _originalHashCode != CurrentValue?.GetHashCode();

        public void Load(T current)
        {
            current.CheckNotNull(nameof(current));

            OriginalValue = _mapper.Map<T>(current);
            CurrentValue = current;

            _originalHashCode = OriginalValue.GetHashCode();
        }

        public void Rollback()
            => _mapper.Map(OriginalValue, CurrentValue);

        public void Update()
            => _mapper.Map(CurrentValue, OriginalValue);
    }
}
