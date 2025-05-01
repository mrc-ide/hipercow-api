// Copyright (c) Imperial College London. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using Prometheus;

/// <summary>
/// Static registry of Hipercow metrics.
/// </summary>
[ExcludeFromCodeCoverage]
public static class MetricsRegistry
{
    /// <summary>
    /// Job count by cluster, user and state.
    /// </summary>
    public static readonly Gauge JobsGauge = Metrics.CreateGauge(
        "jobs",
        "Current jobs running by cluster, user and state.",
        labelNames: ["cluster", "user", "state"]);

    /// <summary>
    /// Core hours used, by cluster and user.
    /// </summary>
    public static readonly Gauge CoreHoursGauge = Metrics.CreateGauge(
        "core_hours",
        "Core hours used by cluster and user.",
        labelNames: ["cluster", "user"]);
}
