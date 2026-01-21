# 本项目 在windows上交叉编译linux程序，需要依赖zig
# 我们后续可以手动在linux 通过strip裁剪调试符号
dotnet publish -c Release -r linux-x64 -p:PublishAot=true -p:StripSymbols=false