using FluentAssertions;
using Library.Domain;
using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class PartyTests
    {

        [Fact]
        public void Create_Valid_Party_With_InitialRole()
        {
            var party = Party.Create("Homer", "homer@library.gr", RoleType.Author);
            party.Name.Should().Be("Homer");
            party.Email.Should().Be("homer@library.gr");
            party.Roles.Should().ContainSingle(r => r.Role == RoleType.Author);
        }

        [Fact]
        public void Create_No_Name_Throws_DomainException()
        {
            var act = () => Party.Create("", "email@test.com", RoleType.Customer);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Update_Updates_Properties()
        {
            var party = Party.Create("Homer", "homer@library.gr", RoleType.Author);
            party.Update("Updated Name", "updated@library.gr");
            party.Name.Should().Be("Updated Name");
            party.Email.Should().Be("updated@library.gr");
        }

        [Fact]
        public void AddRole_AddsNewRole()
        {
            var party = Party.Create("Plato", "plato@library.gr", RoleType.Author);
            party.AddRole(RoleType.Customer);
            party.Roles.Should().HaveCount(2);
            party.Roles.Should().Contain(r => r.Role == RoleType.Customer);
        }

        [Fact]
        public void AddRole_Duplicate_Throws_DomainException()
        {
            var party = Party.Create("Plato", "plato@library.gr", RoleType.Author);
            var act = () => party.AddRole(RoleType.Author);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveRole_RemovesExistingRole()
        {
            var party = Party.Create("Plato", "plato@library.gr", RoleType.Author);
            party.AddRole(RoleType.Customer);
            party.RemoveRole(RoleType.Customer);
            party.Roles.Should().ContainSingle(r => r.Role == RoleType.Author);
        }

        [Fact]
        public void RemoveRole_NonExistentRole_Throws_DomainException()
        {
            var party = Party.Create("Homer", "homer@library.gr", RoleType.Author);
            var act = () => party.RemoveRole(RoleType.Customer);
            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoveRole_LastRole_Throws_DomainException()
        {
            var party = Party.Create("Homer", "homer@library.gr", RoleType.Author);
            var act = () => party.RemoveRole(RoleType.Author);
            act.Should().Throw<DomainException>();
        }
    }
}
