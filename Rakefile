
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
  csc '/t:library /out:bin/DynamicJson.dll /recurse:lib\\\\DynamicJson.cs'
  csc '/t:library /r:bin/DynamicJson.dll   /out:bin/Auth.dll /recurse:src\\\\BasyuraOrg.Twitter\\\\Auth.cs'
  csc '/t:library /r:bin/DynamicJson.dll /r:bin/Auth.dll /out:bin/Twicseratops.dll /recurse:src\\\\BasyuraOrg.Twitter\\\\Twicseratops.cs'
end

task :client do
  csc '/t:exe /r:bin/Twicseratops.dll /out:bin/Client.exe /recurse:sample\\\\Client.cs'
end
