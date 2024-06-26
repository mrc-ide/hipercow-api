// Copyright (c) Imperial College London. All rights reserved.

namespace Hipercow_api_unit_tests.Tools
{
    using Hipercow_api.Tools;
    using Moq;

    /// <summary>
    /// Shared code between tests.
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// A helper to provide a mock HPC Scheduler.
        /// </summary>
        /// <returns>A mocked HPC Scheduler which won't do anything when
        /// Connect is called.</returns>
        public static Mock<IHipercowScheduler> MockScheduler()
        {
            Mock<IHipercowScheduler> fake = new();
            fake.Setup(x => x.Connect(It.IsAny<string>()));
            return fake;
        }
    }
}