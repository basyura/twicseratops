
task :default => "all"

def csc(cmd)
  system "csc /nologo #{cmd}"
end

task :all do
  csc '/t:library DynamicJson.cs'
  csc '/t:library /r:DynamicJson.dll Auth.cs'
  csc '/r:DynamicJson.dll /r:Auth.dll twitter.cs'
end

