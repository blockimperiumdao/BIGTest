using Godot;
using System;
using System.Collections.Generic;
using GodotBlockchain.addons.godotblockchain;
using GodotBlockchain.addons.godotblockchain.utils;
using Thirdweb;

public partial class ui : CanvasLayer
{
	bool isLoginStarted = false;
	
	[Export]
	private ERC20BlockchainContractNode				erc20ContractNode;
	
	[Export]
	private ERC1155BlockchainContractNode			erc1155ContractNode;
	
	[Export]
	private ERC721BlockchainContractNode			erc721ContractNode;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlockchainClientNode.Instance.ClientLogMessage += DisplayLog;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public async void _on_button_pressed()
	{
		var emailEntry = GetNode<LineEdit>("/root/TestScene/CanvasLayer/VBoxContainer/EmailEntry");
		var otpEntry = GetNode<LineEdit>("/root/TestScene/CanvasLayer/VBoxContainer/OTPEntry");
		
		GD.Print("Testing");
		
		if ( isLoginStarted == false )
		{
			BlockchainClientNode.Instance.OnStartLogin( emailEntry.Text );	
			isLoginStarted = true;	
		}
		else
		{
			await BlockchainClientNode.Instance.OnOTPSubmit( otpEntry.Text );
		}
	}
	
	public void _on_smart_wallet_button_pressed()
	{
		BlockchainClientNode.Instance.CreateSmartWallet();
	}
	
	public async void _on_claim_erc20_button_pressed()
	{
		GD.Print("ERC20 Pressed");
		
		if (erc20ContractNode != null)
		{
			erc20ContractNode.Initialize();
			
			ERC20BlockchainContractNode.ERC20TokenMetadata metadata = await erc20ContractNode.FetchMetadata();
			
			GD.Print(metadata.TokenName);
			
			await erc20ContractNode.Claim("1.0");
			
			GD.Print("ERC20 Claimed!");
		}
		else
		{
			GD.Print("No contract node");
		}
	}
	
	public async void _on_claim_erc721_button_pressed()
	{
		GD.Print("ERC721 claim Pressed");
		
		if (erc721ContractNode != null)
		{
			erc721ContractNode.Initialize();

			ERC721BlockchainContractNode.ERC721TokenMetadata metadata = await erc721ContractNode.FetchMetadata();
			
			GD.Print(metadata.TokenName);
			
			await erc721ContractNode.Claim( 1 );
			
			//await erc20ContractNode.Claim("1.0");
			
			GD.Print("ERC721 Claimed!");
		}
		else
		{
			GD.Print("No contract node - did you remember to set the contract node in the Godot UI?");
		}
	}
	
	public async void _on_claim_erc1155_button_pressed()
	{
		GD.Print("ERC1155 claim Pressed");
		
		int tokenId = 1;
		int quantity = 1;
		
		if (erc1155ContractNode != null)
		{
			erc1155ContractNode.Initialize();

			ERC1155BlockchainContractNode.ERC1155TokenMetadata metadata = await erc1155ContractNode.FetchMetadataForToken(tokenId);
			
			GD.Print(metadata.TokenName);
			
			await erc1155ContractNode.Claim( tokenId, quantity );
			
			GD.Print("ERC1155 Claimed!");
		}
		else
		{
			GD.Print("No contract node - did you remember to set the ERC1155 contract node in the Godot UI?");
		}
	}	

	public async void _on_get3dmodel_button_pressed()
	{
		GD.Print("Get 3D Model Pressed");
		
		if (erc721ContractNode != null)
		{
			erc721ContractNode.Initialize();
			
			List<NFT> nfts = await erc721ContractNode.InternalThirdwebContract.ERC721_GetAllNFTs();
			GD.Print("NFTs owned: " + nfts.Count);
			
			//iterate over the NFTs and get the first one
			//
			foreach ( var nft in nfts )
			{
				GD.Print("NFT ID: " + nft.Metadata.Name + " Token ID: " + nft.Metadata.Id );

				if (nft.Metadata.Name.Contains("3d"))
				{
					GD.Print("Fetching state for NFT: " + nft.Metadata.Name + " Token ID: " + nft.Metadata.Id );
					Node nftAsNode = await TokenUtils.GetNFTAsNode( nft );
					
					AddChild(nftAsNode);
				}
			}
			
			//NFT nft = await erc721ContractNode.InternalThirdwebContract.ERC721_GetNFT( tokenId );

			
			GD.Print("3D Model Fetched!");
		}
		else
		{
			GD.Print("No contract node - did you remember to set the contract node in the Godot UI?");
		}
	}
	
	public void DisplayLog( string message )
	{
		GD.Print( message );
	}
}
