
task :default => "all"

def csc(cmd)
  system "csc /nologo #{cmd}"
end

task :all => [:libraries , :twitter] do
end

task :libraries do
  csc '/t:library DynamicJson.cs'
  csc '/t:library /r:DynamicJson.dll Auth.cs'
end

task :twitter do
  csc '/r:DynamicJson.dll /r:Auth.dll twitter.cs'
end
