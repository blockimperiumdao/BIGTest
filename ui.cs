using Godot;
using System;
using GodotBlockchain.addons.godotblockchain;

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
		
		erc20ContractNode.Initialize();
		
		if (erc20ContractNode != null)
		{
			ERC20BlockchainContractNode.ERC20TokenMetadata metadata = await erc20ContractNode.FetchMetadata();
			
			GD.Print(metadata.TokenName);
			
			await erc20ContractNode.Claim("1.0");
		}
		else
		{
			GD.Print("No contract node");
		}
	}
	
	public async void _on_claim_erc721_button_pressed()
	{
		GD.Print("ERC721 claim Pressed");
		
		erc721ContractNode.Initialize();
		
		if (erc721ContractNode != null)
		{
			ERC721BlockchainContractNode.ERC721TokenMetadata metadata = await erc721ContractNode.FetchMetadata();
			
			GD.Print(metadata.TokenName);
			
			await erc721ContractNode.Claim( 1 );
			
			//await erc20ContractNode.Claim("1.0");
		}
		else
		{
			GD.Print("No contract node");
		}
	}
	
	public async void _on_claim_erc1155_button_pressed()
	{
		GD.Print("ERC1155 claim Pressed");
		
		int tokenId = 4;
		int quantity = 1;
		
		erc1155ContractNode.Initialize();
		
		if (erc1155ContractNode != null)
		{
			ERC1155BlockchainContractNode.ERC1155TokenMetadata metadata = await erc1155ContractNode.FetchMetadataForToken(tokenId);
			
			GD.Print(metadata.TokenName);
			
			await erc1155ContractNode.Claim( tokenId, quantity );
		}
		else
		{
			GD.Print("No contract node");
		}
	}	
	
	public void DisplayLog( string message )
	{
		GD.Print( message );
	}
}
