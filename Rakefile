
task :default => "all"

def csc(cmd)
  cmd = "csc /nologo #{cmd}"
  puts cmd
  system cmd
end

task :all => [
  :libraries , :client
] do end

task :libraries do
  csc '/t:library DynamicJson.cs'
  csc '/t:library /r:DynamicJson.dll Auth.cs'
  csc '/t:library /r:DynamicJson.dll /r:Auth.dll twitter.cs'
end

task :client do
  csc '/r:Twitter.dll Client.cs'
end
