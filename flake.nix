{
  description = "Nix flakes for development & buildilng Surya Pamenang career web";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  };

  outputs = {
    self,
    nixpkgs,
  }: let
    pkgs = import nixpkgs {
      system = "x86_64-linux";
      config.allowUnfree = true;
    };
  in {
    devShells.x86_64-linux.default = pkgs.mkShellNoCC {
      packages = with pkgs; [
        just
        dotnet-sdk_10
        dotenv-cli
        openssl
        git-cliff
        commitlint
        lefthook
      ];
    };
  };
}

