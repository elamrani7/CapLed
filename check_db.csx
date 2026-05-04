using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockManager.Infrastructure.Persistence;
using StockManager.Core.Domain.Entities;
using System.Linq;

// This script requires CapLed.Infrastructure to be referenced or compiled.
// I will write a simple raw SQL query instead to avoid dependency hell.
