﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Clanstvo.DataAccess.SqlServer.Data.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbModels = Clanstvo.DataAccess.SqlServer.Data.DbModels;

namespace ClanstvoWebApi.DTOs
{
    public class ClanAggregate
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "First name can't be empty", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "First name can't be longer than 50 characters")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name can't be empty", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Last name can't be longer than 50 characters")]
        public string Prezime { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime DatumRodenja { get; set; }
        public byte[] Slika { get; set; }

        [Required(ErrorMessage = "Address can't be empty", AllowEmptyStrings = false)]
        [StringLength(50, ErrorMessage = "Address can't be longer than 50 characters")]
        public string Adresa { get; set; }
        public bool ImaMaramu { get; set; }
        [Column(TypeName = "date")]
        public DateTime? DatumMarama { get; set; }
        [StringLength(50)]
        [Unicode(false)]
        public string MjestoMarama { get; set; }

        [InverseProperty("Clan")]
        public virtual ICollection<ClanRangStarost> ClanRangStarost { get; set; }
        [InverseProperty("Clan")]
        public virtual ICollection<ClanRangZasluga> ClanRangZasluga { get; set; }
        [InverseProperty("Clan")]
        public virtual ICollection<Clanarina> Clanarina { get; set; }
    }
    public static partial class DtoMapping
    {
        public static ClanAggregate ToAggregateDto(this DbModels.Clan clan)
            => new ClanAggregate()
            {
                Id = clan.Id,
                Ime = clan.Ime,
                Prezime = clan.Prezime,
                Slika = clan.Slika,
                DatumRodenja = clan.DatumRodenja,
                Adresa = clan.Adresa,
                ImaMaramu = clan.ImaMaramu,
                DatumMarama = clan.DatumMarama,
                MjestoMarama = clan.MjestoMarama,
                ClanRangStarost = clan.ClanRangStarost == null
                                ? new List<ClanRangStarost>()
                                : clan.ClanRangStarost.Select(pr => pr.ToDto()).ToList(),
                ClanRangZasluga = clan.ClanRangZasluga == null
                                ? new List<ClanRangZasluga>()
                                : clan.ClanRangZasluga.Select(pr => pr.ToDto()).ToList(),
                Clanarina = clan.Clanarina == null
                                ? new List<Clanarina>()
                                : clan.Clanarina.Select(pr => pr.ToDto()).ToList()
            };

        public static DbModels.Clan ToDbModel(ClanAggregate clan)
            => new DbModels.Clan()
            {
                Id = clan.Id,
                Ime = clan.Ime,
                Prezime = clan.Prezime,
                Slika = clan.Slika,
                DatumRodenja = clan.DatumRodenja,
                Adresa = clan.Adresa,
                ImaMaramu = clan.ImaMaramu,
                DatumMarama = clan.DatumMarama,
                MjestoMarama = clan.MjestoMarama,
                ClanRangStarost = clan.ClanRangStarost.Select(pr => pr.ToDbModel(clan.Id)).ToList(),
                ClanRangZasluga = clan.ClanRangZasluga.Select(pr => pr.ToDbModel(clan.Id)).ToList(),
                Clanarina = clan.Clanarina.Select(pr => pr.ToDbModel()).ToList()
            };
        /*=> new DomainModels.Clan(
                clan.Id,
                clan.Ime,
                clan.Prezime,
                clan.Slika,
                clan.DatumRodenja,
                clan.Adresa,
                clan.ImaMaramu,
                clan.DatumMarama,
                clan.MjestoMarama,
                clan.ClanRangStarost.Select(ToDomain),
                clan.ClanRangZasluga.Select(ToDomain),
                clan.Clanarina.Select(ToDomain)
            )*/
    }
}
