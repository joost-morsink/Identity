using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test
{

    public class Person
    {
        public IIdentity<Person> Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Detail
    {
        public IIdentity<Person, Detail> Id { get; set; }
        public string Description { get; set; }
    }
    public class A
    {
        public IIdentity<A> Id { get; set; }
    }
    public class B
    {
        public IIdentity<B> Id { get; set; }
    }
    public class C
    {
        public IIdentity<C> Id { get; set; }
    }
    public static class DomainExt
    {
        public static IIdentity<Person, Detail> Upgrade(this IIdentity<Detail> id)
        {
            return id as IIdentity<Person, Detail> ?? id.Provider.Translate(id) as IIdentity<Person, Detail>;
        }
    }
}
