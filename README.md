# NetGuard

# What is NetGuard?
NetGuard is my new C# packet filter base for `legend 1(ECSRO, jSRO)` Silkroad Online files, it will most likely work for vSRO 1.88 aswell!

### You may remember me or my previous projects, such as Supermike, Superman, and KRYLFILTER.
[Superman](https://www.elitepvpers.com/forum/sro-pserver-guides-releases/3778943-release-superman-vsro-anti-exploit-free.html)
[Supermike](https://www.elitepvpers.com/forum/sro-pserver-guides-releases/3943130-release-supermike-exploit-filter-free-tons-features-stable.html)

The original engine, async server, and base were initially created by Chernobyl but have been rewritten multiple times since then.

# Why NetGuard?
I set out to create a packet filter with a clean and structured approach to handling packets from both the server and the client. While this version achieves that goal, it is not the final/full release and will keep getting updates daily. The project was ported to `.NET 8.0` from `.NET Framework`, which now supports both Windows and Linux environments.

### Why `.NET 8.0`? 
It brings significant improvements over the older .NET Framework, particularly in terms of performance. `.NET 8.0` introduces native AOT (Ahead-of-Time) compilation, which reduces startup time and improves runtime efficiency by compiling the code directly to native machine code. This leads to faster execution, reduced memory usage, and better resource management.

Additionally, `.NET 8.0` offers enhanced socket handling and optimized networking performance, which is crucial for applications that rely on high-speed data transmission. Its improved garbage collection minimizes memory fragmentation, resulting in smoother performance, especially for long-running applications. These enhancements make `.NET 8.0` more suitable for modern, high-performance environments compared to the older .NET Framework, where these capabilities were either lacking or less efficient.

# Special thanks

* [DaxterSoul](https://www.elitepvpers.com/forum/members/1084164-daxtersoul.html)
  - for the [SilkroadDocs]([https://www.elitepvpers.com/forum/members/1084164-daxtersoul.html](https://github.com/DummkopfOfHachtenduden/SilkroadDoc/))
* [pushedx](https://www.elitepvpers.com/forum/members/900141-pushedx.html)
  - for the original released [SilkroadSecurityAPI](https://www.elitepvpers.com/forum/sro-coding-corner/1063078-c-silkroadsecurity.html)
* Chernobyl
  - for the main idea about building a packet filter
* [DuckSoup](https://github.com/ducksoup-sro/ducksoup)
  - for keeping the spirit up whilst I was gone from the scene
 
# License
Do what you want cause a pirate is free
