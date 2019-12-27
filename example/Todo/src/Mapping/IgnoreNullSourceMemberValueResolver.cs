/*using AutoMapper;

namespace TodoApp.Mapping {
    /// <summary>
    /// MemberValueResolver, resolve to source value if value isnot null. Otherwise return destinationvalue.
    /// </summary>
    public class IgnoreNullSourceMemberValueResolver : IMemberValueResolver<object, object, object, object> {
        public object Resolve(object source, object destination, object sourceMember, object destinationMember, ResolutionContext context) {
            return sourceMember ?? destinationMember;
        }
    }
}*/