using Godot;
using System;
using System.Collections.Generic;
using GodotBlockchain.addons.godotblockchain;
using GodotBlockchain.addons.godotblockchain.utils;
using Thirdweb;

public partial class ui : CanvasLayer
{
	[Signal]
	public delegate void ChangeMenuModeEventHandler( bool isMenuMode );

	bool isLoginStarted = false;
	
	[Export]
	private ERC20BlockchainContractNode				erc20ContractNode;
	
	[Export]
	private ERC1155BlockchainContractNode			erc1155ContractNode;
	
	[Export]
	private ERC721BlockchainContractNode			erc721ContractNode;

	private VBoxContainer                           _uiNode;

	private CharacterBody3D 						_characterBody3D;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlockchainClientNode.Instance.ClientLogMessage += DisplayLog;

		_uiNode = GetNode<VBoxContainer>("CenterContainer/VBoxContainer");
		_characterBody3D = GetNode<CharacterBody3D>("/root/TestScene/CharacterBody3D");

		if (_characterBody3D != null)
		{
			Callable callable = new Callable(_characterBody3D, "on_menu_mode_changed");
			// Connect the signal to the method
			Connect( "ChangeMenuMode", callable );
		}

		// if (_characterBody3D is FirstPersonController firstPersonController) 
		// 	ChangeMenuMode += firstPersonController.OnMenuModeChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel")) // Default "Escape" key action in Godot
		{
			// Toggle visibility
			if (_uiNode != null)
			{
				_uiNode.Visible = !_uiNode.Visible;

				// Emit signal to notify other nodes that the menu mode has changed
				EmitSignal( SignalName.ChangeMenuMode, _uiNode.Visible );
			}
		}
	}
	
	public async void _on_button_pressed()
	{
		var emailEntry = GetNode<LineEdit>("/root/TestScene/CanvasLayer/CenterContainer/VBoxContainer/EmailEntry");
		var otpEntry = GetNode<LineEdit>("/root/TestScene/CanvasLayer/CenterContainer/VBoxContainer/OTPEntry");
		
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

	public async void _on_getNftAsSprite2D_button_pressed()
	{
		GD.Print("Get NFT as Sprite2D Pressed");

		var tokenId = 0;
		
		if (erc721ContractNode != null)
		{
			erc721ContractNode.Initialize();
			
			NFT nft = await TokenUtils.GetNFTFromContractNode( erc721ContractNode, tokenId );
			Sprite2D nftAsSprite2D = await TokenUtils.GetNFTAsSprite2D(nft);

			AddChild(nftAsSprite2D);
		}
	}
	
	public async void _on_getNftAsTexture_button_pressed()
	{
		GD.Print("Get NFT as Texture Pressed");

		var tokenId = 0;
		
		if (erc721ContractNode != null)
		{
			erc721ContractNode.Initialize();
			
			MeshInstance3D box = new MeshInstance3D();
			box.Mesh = new BoxMesh();

			var material = new StandardMaterial3D();
	
			NFT nft = await TokenUtils.GetNFTFromContractNode( erc721ContractNode, tokenId );
			ImageTexture nftAsImageTexture = await TokenUtils.GetNFTAsTexture(nft);

			material.AlbedoTexture = nftAsImageTexture;
			box.Mesh.SurfaceSetMaterial(0, material);
			
			AddChild(box);
			
			box.SetGlobalPosition(new Vector3(0.0f, 0.0f, -10.0f) );
		}
	}

	public async void _on_getNftAsAudio_button_pressed()
	{
		GD.Print("Get NFT as Audio Pressed");

		var tokenId = 3;
		
		if (erc721ContractNode != null)
		{
			erc721ContractNode.Initialize();
			
			NFT nft = await TokenUtils.GetNFTFromContractNode( erc721ContractNode, tokenId );
			AudioStream nftAsAudio = await TokenUtils.GetNFTAsAudioStreamMP3(nft);
			
			AudioStreamPlayer audioStreamPlayer = new AudioStreamPlayer();
			AddChild(audioStreamPlayer);
			
			audioStreamPlayer.Stream = nftAsAudio;
			audioStreamPlayer.Play();
		}
	}

	public async void _on_get3dmodel_button_pressed()
	{
		GD.Print("Get 3D Model Pressed");
		
		if (erc721ContractNode != null)
		{
			erc721ContractNode.Initialize();

			List<NFT> aList = await TokenUtils.QueryOwnedNFTsFromContractNode(erc721ContractNode, new Dictionary<string, object>());
			
			List<NFT> nfts = await TokenUtils.GetAllNFTsFromContractNode( erc721ContractNode );
			GD.Print("NFTs owned: " + nfts.Count);
			
			//iterate over the NFTs and get the first one
			//
			foreach ( var nft in nfts )
			{
				GD.Print("NFT ID: " + nft.Metadata.Name + " Token ID: " + nft.Metadata.Id );

				if (nft.Metadata.Name.Contains("Scene"))
				{
					GD.Print("Fetching state for NFT: " + nft.Metadata.Name + " Token ID: " + nft.Metadata.Id );
					Node nftAsNode = await TokenUtils.GetNFTAsNode( nft );
					AddChild(nftAsNode);
					
					GD.Print("3D Model Fetched!");

				}
			}
			
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
